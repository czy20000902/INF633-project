using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Animal : MonoBehaviour
{
    // Specify the detection range for food.
    [Header("Food Detection")]
    public float foodDetectionRange = 5.0f; 

    public enum Gender { Male, Female }
    public Gender gender;

    public float reproductionEnergyThreshold = 20.0f; // 能量达到多少才能繁殖
    public float reproductionDistance = 5.0f; // 雌雄相遇的最大距离
    public float reproductionCooldown = 10.0f; // 繁殖冷却时间
    private float lastReproductionTime;

    public Material maleMaterial; // 雄性的材质
    public Material femaleMaterial; // 雌性的材质


    [Header("Animal parameters")]
    public float swapRate = 0.01f;
    public float mutateRate = 0.01f;
    public float swapStrength = 10.0f;
    public float mutateStrength = 0.5f;
    public float maxAngle = 10.0f;

    [Header("Energy parameters")]
    public float maxEnergy = 10.0f;
    public float lossEnergy = 0.1f;
    public float gainEnergy = 10.0f;
    private float energy;

    [Header("Sensor - Vision")]
    public float maxVision = 20.0f;
    public float stepAngle = 10.0f;
    public int nEyes = 5;

    private int[] networkStruct;
    private SimpleNeuralNet brain = null;

    // Terrain.
    private CustomTerrain terrain = null;
    private int[,] details = null;
    private Vector2 detailSize;
    private Vector2 terrainSize;

    // Animal.
    private Transform tfm;
    private float[] vision;

    // Genetic alg.
    private GeneticAlgo genetic_algo = null;

    // Renderer.
    private Material mat = null;

    // 设置动物的材质
    private void SetMaterial(Material material)
    {
        MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }
    }

    void Start()
    {
        // Network: 1 input per receptor, 1 output per actuator.
        vision = new float[nEyes];
        networkStruct = new int[] { nEyes, 5, 1 };
        energy = maxEnergy;
        tfm = transform;

        // Renderer used to update animal color.
        // It needs to be updated for more complex models.
        MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
        if (renderer != null)
            mat = renderer.material;

        // 随机性别
        gender = (UnityEngine.Random.value < 0.5f) ? Gender.Male : Gender.Female;

        if (gender == Gender.Male)
        {
            SetMaterial(maleMaterial);
        }
        else
        {
            SetMaterial(femaleMaterial);
        }

    }

    private void ReproduceWith(Animal partner)
    {
        // 复制大脑
        SimpleNeuralNet newBrain = new SimpleNeuralNet(GetBrain());

        // 随机分配性别
        Gender newGender = (UnityEngine.Random.value < 0.5f) ? Gender.Male : Gender.Female;

        // 创建新的动物
        GameObject newAnimal = genetic_algo.makeAnimal();
        Animal newAnimalScript = newAnimal.GetComponent<Animal>();
        newAnimalScript.InheritBrain(newBrain, true);
        newAnimalScript.gender = newGender;

        if (gender == Gender.Male)
        {
            SetMaterial(maleMaterial);
        }
        else
        {
            SetMaterial(femaleMaterial);
        }
    }

    private void CheckReproduction()
{
    if (gender == Gender.Female && energy >= reproductionEnergyThreshold)
    {
        foreach (GameObject otherAnimal in genetic_algo.animals)
        {
            Animal other = otherAnimal.GetComponent<Animal>();
            if (other != null && other != this && other.gender == Gender.Male &&
                Vector3.Distance(transform.position, other.transform.position) <= reproductionDistance)
            {
                if (Time.time - lastReproductionTime >= reproductionCooldown)
                {
                    ReproduceWith(other);
                    lastReproductionTime = Time.time;
                }
            }
        }
    }
}

    void Update()
    {
        // In case something is not initialized...
        if (brain == null)
            brain = new SimpleNeuralNet(networkStruct);
        if (terrain == null)
            return;
        if (details == null)
        {
            UpdateSetup();
            return;
        }

        // Retrieve animal location in the heighmap
        int dx = (int)((tfm.position.x / terrainSize.x) * detailSize.x);
        int dy = (int)((tfm.position.z / terrainSize.y) * detailSize.y);

        // For each frame, we lose lossEnergy
        energy -= lossEnergy;

        // If the animal is located in the dimensions of the terrain and over a grass position (details[dy, dx] > 0), it eats it, gain energy and spawn an offspring.
        if ((dx >= 0) && dx < (details.GetLength(1)) && (dy >= 0) && (dy < details.GetLength(0)) && details[dy, dx] > 0)
        {
            // Eat (remove) the grass and gain energy.
            details[dy, dx] = 0;
            energy += gainEnergy;
            if (energy > maxEnergy)
                energy = maxEnergy;

            genetic_algo.addOffspring(this);
        }

        // If the energy is below 0, the animal dies.
        if (energy < 0)
        {
            energy = 0.0f;
            genetic_algo.removeAnimal(this);
        }

        // Check for reproduction.
        CheckReproduction();
    

        // Update the color of the animal as a function of the energy that it contains.
        if (mat != null)
            mat.color = Color.white * (energy / maxEnergy);

        // 1. Update receptor.
        UpdateVision();

        // 2. Use brain.
        float[] output = brain.getOutput(vision);

        // 3. Act using actuators.
        float angle = (output[0] * 2.0f - 1.0f) * maxAngle;
        tfm.Rotate(0.0f, angle, 0.0f);
    }

    /// <summary>
    /// Calculate distance to the nearest food resource, if there is any.
    /// </summary>
    private void UpdateVision()
    {
        float startingAngle = -((float)nEyes / 2.0f) * stepAngle;
        Vector2 ratio = detailSize / terrainSize;

        for (int i = 0; i < nEyes; i++)
        {
            Quaternion rotAnimal = tfm.rotation * Quaternion.Euler(0.0f, startingAngle + (stepAngle * i), 0.0f);
            Vector3 forwardAnimal = rotAnimal * Vector3.forward;
            float sx = tfm.position.x * ratio.x;
            float sy = tfm.position.z * ratio.y;
            vision[i] = 1.0f;

            // Interate over vision length.
            for (float distance = 1.0f; distance < maxVision; distance += 0.5f)
            {
                // Position where we are looking at.
                float px = (sx + (distance * forwardAnimal.x * ratio.x));
                float py = (sy + (distance * forwardAnimal.z * ratio.y));

                if (px < 0)
                    px += detailSize.x;
                else if (px >= detailSize.x)
                    px -= detailSize.x;
                if (py < 0)
                    py += detailSize.y;
                else if (py >= detailSize.y)
                    py -= detailSize.y;

                if ((int)px >= 0 && (int)px < details.GetLength(1) && (int)py >= 0 && (int)py < details.GetLength(0) && details[(int)py, (int)px] > 0)
                {
                    vision[i] = distance / maxVision;
                    break;
                }
            }
        }
    }

    public void Setup(CustomTerrain ct, GeneticAlgo ga)
    {
        terrain = ct;
        genetic_algo = ga;
        UpdateSetup();
    }

    private void UpdateSetup()
    {
        detailSize = terrain.detailSize();
        Vector3 gsz = terrain.terrainSize();
        terrainSize = new Vector2(gsz.x, gsz.z);
        details = terrain.getDetails();
    }

    public void InheritBrain(SimpleNeuralNet other, bool mutate)
    {
        brain = new SimpleNeuralNet(other);
        if (mutate)
            brain.mutate(swapRate, mutateRate, swapStrength, mutateStrength);
    }
    public SimpleNeuralNet GetBrain()
    {
        return brain;
    }
    public float GetHealth()
    {
        return energy / maxEnergy;
    }

}

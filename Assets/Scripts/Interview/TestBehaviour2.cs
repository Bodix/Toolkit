using UnityEngine;

public class TestBehaviour2 : MonoBehaviour
{
    private int[] GetMultiplications(int[] numbers)
    {
        int allMultiplied = 1;
        for (int i = 0; i < numbers.Length; i++)
        {
            if (numbers[i] == 0)
                continue;

            allMultiplied *= numbers[i];
        }

        int[] result = new int [numbers.Length];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = allMultiplied / numbers[i];
        }

        return result;
    }
}
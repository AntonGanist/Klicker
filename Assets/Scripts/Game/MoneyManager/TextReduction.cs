using UnityEngine;

public static class TextReduction
{
    public static string Reduction(float number)
    {
        string txt = "";
        string formattedValue = "0";
        int del = 1;

        if (number > 9)
        {
            if (number >= 1000000000)
            {
                txt = " לכנה";
                del = 1000000000;
            }
            else if (number >= 1000000)
            {
                txt = " לכם";
                del = 1000000;
            }
            else if (number >= 1000)
            {
                txt = " עûס";
                del = 1000;
            }

            float currentMoney = number / del;
            if (currentMoney != 0)
            {
                if (currentMoney >= 10f)
                {
                    formattedValue = Mathf.RoundToInt(currentMoney).ToString();
                }
                else
                {
                    formattedValue = currentMoney.ToString("0.0");
                }
            }
        }
        else
        {
            formattedValue = number.ToString();
        }
        return formattedValue + txt;
    }
}

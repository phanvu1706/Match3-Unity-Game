using System;
//using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
using URandom = UnityEngine.Random;

public class Utils
{
    public static NormalItem.eNormalType GetRandomNormalType()
    {
        Array values = Enum.GetValues(typeof(NormalItem.eNormalType));
        NormalItem.eNormalType result = (NormalItem.eNormalType)values.GetValue(URandom.Range(0, values.Length));

        return result;
    }

    public static NormalItem.eNormalType GetRandomNormalTypeExcept(List<NormalItem.eNormalType> types)
    {
        //List<NormalItem.eNormalType> list = Enum.GetValues(typeof(NormalItem.eNormalType)).Cast<NormalItem.eNormalType>().Except(types).ToList();
        List<NormalItem.eNormalType> list = ListPool<NormalItem.eNormalType>.Get();
        list.Clear(); int totalTypeCount = Enum.GetValues(typeof(NormalItem.eNormalType)).Length;
        for (int i = 0; i < totalTypeCount; ++i)
        {
            NormalItem.eNormalType type = (NormalItem.eNormalType)i;
            if (!types.Contains(type)) list.Add(type);
        }

        int rnd = URandom.Range(0, list.Count);
        NormalItem.eNormalType result = list[rnd];

        list.Clear();
        ListPool<NormalItem.eNormalType>.Release(list);
        return result;
    }

    public static NormalItem.eNormalType GetLeastAmountNormalTypeExcept(List<NormalItem.eNormalType> types, List<int> typesCount)
    {
        List<NormalItem.eNormalType> list = ListPool<NormalItem.eNormalType>.Get();
        list.Clear(); int totalTypeCount = Enum.GetValues(typeof(NormalItem.eNormalType)).Length;
        for (int i = 0; i < totalTypeCount; ++i)
        {
            NormalItem.eNormalType type = (NormalItem.eNormalType)i;
            if (!types.Contains(type)) list.Add(type);
        }

        int index = 0;
        int least = typesCount[(int)list[index]];

        for (int i = 1; i < list.Count; ++i)
        {
            int count = typesCount[(int)list[i]];
            if (count < least)
            {
                least = count;
                index = i;
            }
        }
        NormalItem.eNormalType result = list[index];

        list.Clear();
        ListPool<NormalItem.eNormalType>.Release(list);
        return result;
    }
}

using System;
using UnityEngine;

public static class ResourceService
{
    public static event Action<ResourceType, int> OnResourceChanged;

    // -------------------------
    // GLOBAL RESOURCES
    // -------------------------
    public static void Add(ResourceType type, int amount)
    {
        var data = GameState.Instance.Data;

        switch (type)
        {
            case ResourceType.Money:
                data.money += amount;
                break;
            case ResourceType.Fuel:
                data.fuel += amount;
                break;
            case ResourceType.Orbs:
                data.orbs += amount;
                break;
            case ResourceType.Missiles:
                data.missiles += amount;
                break;

            default:
                Debug.LogError($"{type} requires a CharacterData target");
                return;
        }

        OnResourceChanged?.Invoke(type, amount);
    }

    // -------------------------
    // CHARACTER RESOURCES
    // -------------------------
    public static void Add(ResourceType type, int amount, CharacterData target)
    {
        if (target == null)
        {
            Debug.LogError("Character resource requires CharacterData target");
            return;
        }

        switch (type)
        {
            case ResourceType.Health:
                target.healthData += amount;
                break;
            case ResourceType.Power:
                target.powerData += amount;
                break;
            case ResourceType.Graze:
                target.grazeData += amount;
                break;
            case ResourceType.Bombs:
                target.bombsData += amount;
                break;

            default:
                Debug.LogError($"{type} is not a character resource");
                return;
        }

        OnResourceChanged?.Invoke(type, amount);
    }
}
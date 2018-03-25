using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

/// <summary>
/// Fonctions utile
/// <summary>
public static class GameData
{
    #region core script

    public enum Event
    {
        PlayerDeath,    //est appelé a chaque playerDeath
        GameOver,       //est appelé quand on trigger un gameOver
        GamePadConnectionChange,    //est appelé a chaque co/deco de manette
        AdditiveJustFinishLoad,
        SuperPower
    };

    public enum Prefabs
    {
        Link,
        Player,
        SuperPower,
        Bumper,
    };

    public enum PoolTag
    {
        ParticleBump,
        Link,
        DeathPlayer,
    }

    public enum Layers
    {
        Player,
        Obstacle,
        Default,
    }

    public enum Sounds
    {
        Bump,
        SpiksOn,
        SpiksOff,
    }

    /// <summary>
    /// retourne vrai si le layer est dans la list
    /// </summary>
    public static bool IsInList(List<Layers> listLayer, int layer)
    {
        string layerName = LayerMask.LayerToName(layer);
        for (int i = 0; i < listLayer.Count; i++)
        {
            if (listLayer[i].ToString() == layerName)
            {
                return (true);
            }
        }
        return (false);
    }
    /// <summary>
    /// retourne vrai si le layer est dans la list
    /// </summary>
    public static bool IsInList(List<Prefabs> listLayer, string tag)
    {
        for (int i = 0; i < listLayer.Count; i++)
        {
            if (listLayer[i].ToString() == tag)
            {
                return (true);
            }
        }
        return (false);
    }
    #endregion
}

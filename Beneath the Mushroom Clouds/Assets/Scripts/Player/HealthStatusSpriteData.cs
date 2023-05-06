using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object holding information about different injury and treatment textures for differen body parts in the health screen.
/// </summary>
[CreateAssetMenu]
public class HealthStatusSpriteData : ScriptableObject
{
    /// <summary>
    /// Bleeding injury texture for the body part.
    /// </summary>
    public Texture2D bleeding;

    /// <summary>
    /// Clean bandage texture for the body part.
    /// </summary>
    public Texture2D bandageClean;

    /// <summary>
    /// Dirty bandage texture for the body part.
    /// </summary>
    public Texture2D bandageDirty;
    /// <summary>
    /// Stitched wound texture for the body part.
    /// </summary>
    public Texture2D stitchedWound;
    /// <summary>
    /// Gunshot wound texture for the body part.
    /// </summary>
    public Texture2D gunshotWound;
    /// <summary>
    /// Infection texture for the body part.
    /// </summary>
    public Texture2D infection;

}

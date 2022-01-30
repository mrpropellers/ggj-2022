using System;
using System.Collections;
using GGJ;
using GGJ.Utility.Runtime.Utility;
using UnityEngine;

// TODO? Might want to put this class on the same object as StageState or even merge it with the StageState instance

/// <summary>
/// This script attaches to the Main camera and helps simplify management of any vfx that we want to implement
/// as full-screen sprite renderers
/// </summary>
[RequireComponent(typeof(Camera))]
public class EffectsManager : MonoBehaviour
{
    Camera m_Camera;
    BoardPiece[] m_AllBoardPieces;

    [SerializeField]
    [Range(0f, 1f)]
    float m_TransitionTime;

    [SerializeField]
    SpriteRenderer m_FogEffect;

    // TODO: This should be defined as a ScriptableObject and which each Piece can reference -- some pieces
    //       may not want to use the default set
    [SerializeField]
    Material m_TangiblePieceMaterial;
    [SerializeField]
    Material m_EtherealPieceMaterial;

    void Start()
    {
        m_AllBoardPieces = FindObjectsOfType<BoardPiece>();
        InitializeEffects();
        UnityHelpers.DoCoroutineImmediately(SetRealmEffects());
    }

    void OnValidate()
    {
        InitializeEffects();
        if (m_TransitionTime < 0f)
        {
            Debug.LogWarning("Transition time can't be negative");
            m_TransitionTime = 0f;
        }
    }

    void InitializeEffects()
    {
        m_Camera = GetComponent<Camera>();

        CameraHelpers.ResizeSpriteToFitScreen(m_Camera, m_FogEffect);
    }


    void SetAllBoardPieceMaterials(StageState.BoardMode mode)
    {
        foreach (var piece in m_AllBoardPieces)
        {
            var renderer = piece.GetComponent<SpriteRenderer>();
            if (StageState.Instance.IsTangible(piece))
            {
                renderer.material = m_TangiblePieceMaterial;
            }
            else
            {
                renderer.material = m_EtherealPieceMaterial;
            }
        }
    }

    IEnumerator TransitionTo(StageState.BoardMode mode)
    {
        var mask = m_FogEffect.GetComponent<SpriteMask>();
        var totalDelta = mode == StageState.BoardMode.Physical ? 1f : -1f;
        var originalCutoff = mask.alphaCutoff;
        var tAccumulated = 0f;
        while (tAccumulated < 1f)
        {
            var t = Time.deltaTime / m_TransitionTime;
            tAccumulated += t;
            mask.alphaCutoff = originalCutoff + totalDelta * tAccumulated;
            yield return null;
        }

        mask.alphaCutoff = Mathf.Clamp01(originalCutoff + totalDelta * tAccumulated);
    }

    public IEnumerator SetRealmEffects()
    {
        var mode = StageState.Instance.CurrentBoardMode;
        // NOTE: This can be handled via the sprite mask
        //switch (mode)
        //{
        //    case StageState.BoardMode.Physical:
        //        m_FogEffect.enabled = false;
        //        break;
        //    case StageState.BoardMode.Spiritual:
        //        m_FogEffect.enabled = true;
        //        break;
        //    default:
        //        throw new ArgumentOutOfRangeException($"No handling for {mode}");
        //}
        yield return TransitionTo(mode);
        SetAllBoardPieceMaterials(mode);
    }
}

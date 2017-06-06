using System;
using UnityEngine;

public class SpikePlatform : MonoBehaviour
{
    public enum State
    {
        In, Out, Spiking, Contraction
    }
    
    [SerializeField]
    protected Transform[] spikes;

    [SerializeField]
    protected float spikeDuration,
        contractDuration,
        inDuration,
        outDuration;

    [SerializeField]
    protected State startState;

    [SerializeField]
    protected AudioSource audioSource;

    private State state;

    private DateTime inTime, outTime;
    private float spikingCount = 0, contractionCount;
    private float spikeMaxScale;

    private void Start()
    {
        state = startState;
        spikeMaxScale = spikes[0].localScale.y;

        switch (state)
        {
            case State.In:
                SetSpikesYScale(0.0f);
                outTime = DateTime.Now.AddSeconds(inDuration);
                break;
            case State.Out:
                SetSpikesYScale(spikeMaxScale);
                inTime = DateTime.Now.AddSeconds(outDuration);
                break;
            case State.Spiking:
                SetSpikesYScale(0.0f);
                spikingCount = 0.0f;
                break;
            case State.Contraction:
                SetSpikesYScale(spikeMaxScale);
                contractionCount = 0.0f;
                break;
        }
    }

    private void Update()
    {
        switch (state)
        {
            case State.In:
                if (outTime <= DateTime.Now)
                {
                    state = State.Spiking;
                    spikingCount = 0.0f;
                    audioSource.Play();
                }
                break;
            case State.Out:
                if (inTime <= DateTime.Now)
                {
                    state = State.Contraction;
                    contractionCount = 0.0f;
                }
                break;
            case State.Spiking:
                spikingCount += Time.deltaTime;
                ExtractSpikes();
                if (spikingCount >= spikeDuration)
                {
                    state = State.Out;
                    inTime = DateTime.Now.AddSeconds(outDuration);
                }
                break;
            case State.Contraction:
                contractionCount += Time.deltaTime;
                ContractSpikes();
                if (contractionCount >= contractDuration)
                {
                    state = State.In;
                    outTime = DateTime.Now.AddSeconds(inDuration);
                }
                break;
        }
    }

    private void ExtractSpikes()
    {
        SetSpikesYScale(Mathf.Lerp(0, spikeMaxScale, spikingCount / spikeDuration));
    }
    
    private void ContractSpikes()
    {
        SetSpikesYScale(Mathf.Lerp(spikeMaxScale, 0, contractionCount / contractDuration));
    }

    private void SetSpikesYScale(float y)
    {
        int length = spikes.Length;
        for (int i = 0; i < length; i++)
        {
            var spike = spikes[i];
            var scale = spike.localScale;
            scale.y = y;
            spike.localScale = scale;
        }
    }
}

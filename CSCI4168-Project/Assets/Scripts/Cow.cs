﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : MonoBehaviour
{
    AudioSource audio;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // check if this cow was clicked
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit[] hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform == this.transform)
                {
                    StartCoroutine(Moo());
                }
            }
        }
    }

    public IEnumerator Moo()
    {
        audio?.Play();

        while (audio?.isPlaying ?? false)
        {
            yield return new WaitForEndOfFrame();
        }
    }
}
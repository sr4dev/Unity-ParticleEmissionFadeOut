using UnityEngine;

public class Example : MonoBehaviour
{
    private void OnGUI()
    {
        if (GUILayout.Button("Turn On Emissions"))
        {
            // turn on all emissions
            foreach (var pec in FindObjectsOfType<ParticleEmissionController>())
            {
                pec.Play();
            }
        }
        
        if (GUILayout.Button("Turn Off Emissions - without acceleration"))
        {
            // turn off all emissions without acceleration
            foreach (var pec in FindObjectsOfType<ParticleEmissionController>())
            {
                pec.Stop(false);
            }
        }

        if (GUILayout.Button("Turn Off Emissions - with acceleration"))
        {
            // turn off all emissions with acceleration
            foreach (var pec in FindObjectsOfType<ParticleEmissionController>())
            {
                pec.Stop();
            }
        }
    }
}
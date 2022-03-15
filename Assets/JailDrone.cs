using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class JailDrone : MonoBehaviour
{

    AudioSource alarm;

    public bool followPlayer = false;

    public float speed;
    float wpRadius = 1;
    public float damping;

    public Transform player;
    public Transform vaultTrig;



    // Start is called before the first frame update
    void Start()
    {
        alarm = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player.transform.position.y < vaultTrig.transform.position.y)
        {
            followPlayer = true;
        }
        
        if (followPlayer == true)
        {
            var rotation = Quaternion.LookRotation(player.transform.position - transform.position);
            alarm.enabled = true;
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * speed);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
        }
        else
        {
            alarm.enabled = false;
        }

        if(Vector3.Distance(player.transform.position, transform.position) < wpRadius)
        {
            //LOAD NEXT SCENE LOGIC


            SceneManager.LoadScene("3.2JailScene");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresenceOfOthers : MonoBehaviour
{
    public static int avatarCount = 20;
    private List<Avatar> avatars = new List<Avatar>();


    public GameObject avatarGraphic;


    void Start()
    {
        for (int i = 0; i < avatarCount; i++)
        {
            Avatar avatar = new Avatar(avatarGraphic);
            avatars.Add(avatar);

        }
    }

    void Update()
    {
        foreach (Avatar avatar in avatars)
        {
            avatar.Update();
        } 
    }
}

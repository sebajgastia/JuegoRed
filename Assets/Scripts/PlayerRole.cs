using UnityEngine;

public class PlayerRole : MonoBehaviour
{
    public enum Role
    {
        Hider,
        Seeker
    }

    [SerializeField] private Role currentRole;

    public Role CurrentRole => currentRole;

    public void SetRole(Role newRole)
    {
        currentRole = newRole;

        Debug.Log(gameObject.name + " ahora es " + currentRole);
    }
}
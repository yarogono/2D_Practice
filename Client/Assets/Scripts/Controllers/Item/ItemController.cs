using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private enum Type { Ammo, Coin, Grenade, Heart, Weapon };
    [SerializeField] private Type _type;
    [SerializeField] private int _value;

    private void Update()
    {
        transform.Rotate(Vector3.up * 10 * Time.deltaTime);
    }
}
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    private string _name { get; set;}
    private string _description { get; set; }
    private int _cuality { get; set; }

    public abstract void ApplyEffect(PlayerController player);
}

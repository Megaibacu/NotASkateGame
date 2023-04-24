using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoRRAReJEMPLOrODRI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Animal a1;
        Dog d1 = new Dog();
        Cat c1 = new Cat();

        a1 = c1;
        a1.Roar();

    }

}

public abstract class Animal {
    public string name;
    public virtual void Roar() {
        Debug.Log("Default Sound");
    }
}

public class Dog : Animal{
    public override void Roar()
    {
        Debug.Log("Bark");
    }
}
public class Cat : Animal
{
    public override void Roar()
    {
        Debug.Log("Mew");
    }
}
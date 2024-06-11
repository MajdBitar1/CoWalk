using UnityEngine;


class CubeLook : MonoBehaviour
{
    public Color Select;
    public Color UnSelect;
    private bool _isLooked;

    public void Looked(bool looked)
    {
        this._isLooked = looked;
        if (looked)
        {
            base.GetComponent<MeshRenderer>().material.color = this.Select;
        }
        else
        {
            base.GetComponent<MeshRenderer>().material.color = this.UnSelect;
        }
    }

    void Update()
    {
        if (!this._isLooked)
        {
            this.Looked(false);
        }
        else
        {
            this._isLooked = false;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprites : MonoBehaviour
{
    //[SerializeField] private Sprite vampireSprite;
    //[SerializeField] private Sprite wolfSprite;
    //[SerializeField] private Sprite batSprite;
    //[SerializeField] private Sprite fogSprite;

    private Animator animator;
    private Player player;
    private BoxCollider2D collider;
    private Player.Form lastKnownForm = Player.Form.Vampire;

    [SerializeField] FormBounds[] formBounds;

    [System.Serializable]
    private struct FormBounds
    {
        public Player.Form Form;
        public Vector2 Size;
        public Vector2 Offset;
    }
    private void Awake()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        lastKnownForm = player.CurrentForm;
    }

    // Update is called once per frame
    void Update()
    {
        if (lastKnownForm != player.CurrentForm)
        {
            UpdateSprite(player.CurrentForm);
            lastKnownForm = player.CurrentForm;
        }
    }

    private void UpdateSprite(Player.Form form)
    {
        switch (form)
        {
            case (Player.Form.Vampire):
                animator.Play("Walking");
                //SetSprite(vampireSprite);
                break;
            case (Player.Form.Bat):
                animator.Play("Flying");
                break;
            case (Player.Form.Wolf):
                animator.Play("Jump");
                break;
            case (Player.Form.Fog):
                animator.Play("Fog");
                break;
            case (Player.Form.Stun):
                animator.Play("Stun");
                break;
        }

        UpdateColliderBox(form);
    }

    private void UpdateColliderBox(Player.Form form)
    {
        FormBounds fb = GetFormBounds(form);
        
        collider.offset = fb.Offset;
        collider.size = fb.Size;
    }

    private void SetSprite(Sprite sprite)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    private FormBounds GetFormBounds(Player.Form form)
    {
        for (int i = 0; i < formBounds.Length; i++)
        {
            if (formBounds[i].Form == form )
            {
                return formBounds[i];
            }
        }

        Debug.Log("error! form not found" + form);
        return formBounds[0];
    }
}

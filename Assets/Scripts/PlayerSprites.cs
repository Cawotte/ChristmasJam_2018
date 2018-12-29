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
    private Player.Form lastKnownForm = Player.Form.Vampire;

    private void Awake()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
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
                //SetSprite(batSprite);
                break;
            case (Player.Form.Wolf):
                animator.Play("Jump");
                //SetSprite(wolfSprite);
                break;
            case (Player.Form.Fog):
                animator.Play("Fog");
                //SetSprite(fogSprite);
                break;
        }
    }

    private void SetSprite(Sprite sprite)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
}

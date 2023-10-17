using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] Animator animator;
    [SerializeField] IdlePlayer player;

    [SerializeField] FixedTouchField touchField;
    [SerializeField] private NavMeshAgent agent;

    [SerializeField] ParticleSystem leftFootStepDust;
    [SerializeField] ParticleSystem rightFootStepDust;



    private Vector3 moveDir;
    private float angle;
    public float moveSpeed;
    public float extraSpeed = 0;

    public float GetCurrentMoveSpeed() => touchField.distBetweenJoystickBodyToHandle;

    private void Start()
    {
        if (ES3.KeyExists("PlayerPos"))
            agent.Warp(ES3.Load<Vector3>("PlayerPos"));

        this.TaskWhile(2f, 0, () => SavePlayerPos());

        player = GetComponent<IdlePlayer>();
    }


    void Update()
    {
        moveDir = touchField.joystickDir.normalized * touchField.distBetweenJoystickBodyToHandle;

        float nomalizeMoveSpeed = touchField.distBetweenJoystickBodyToHandle;

        var delta = new Vector3(moveDir.x, 0, moveDir.y) * (moveSpeed + extraSpeed) * Time.deltaTime;
        agent.Move(delta);

        if (nomalizeMoveSpeed == 0)
            animator.SetBool("Move", false);
        else
            animator.SetBool("Move", true);

        if (player.itemStackList.Count > 0)
            animator.SetLayerWeight(1, 1);
        else
            animator.SetLayerWeight(1, 0);

        if (Mathf.Abs(touchField.joystickDir.normalized.x) > 0 || Mathf.Abs(touchField.joystickDir.normalized.y) > 0)
        {
            angle = Mathf.Atan2((touchField.joystickDir.normalized.y + transform.position.y) - transform.position.y,
            (touchField.joystickDir.normalized.x + transform.position.x) - transform.position.x) * Mathf.Rad2Deg;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle - 90, Vector3.down), 10 * Time.deltaTime);
    }

    private void SavePlayerPos()
    {
        ES3.Save<Vector3>("PlayerPos", transform.position);
    }

    public void SetPlayerMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void PlayLeftFootStepParticle()
    {
        leftFootStepDust.Play();
    }

    public void PlayRightFootStepParticle()
    {
        rightFootStepDust.Play();
    }


}

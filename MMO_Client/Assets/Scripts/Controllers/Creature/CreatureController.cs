﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureController : MonoBehaviour
{
    public float _speed = 5.0f;

    public Vector3Int CellPos { get; set; } = Vector3Int.zero;
    protected Animator _animator;
    protected SpriteRenderer _sprite;
    CreatureState _state = CreatureState.Idle;
    public CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;
            _state = value;

            UpdateAnimation();
        }
    }

    MoveDirection _lastDir = MoveDirection.Down; // 최근에 이동한 방향
    MoveDirection _moveDir = MoveDirection.Down; // 현재 이동 방향
    protected MoveDirection Dir
    {
        get { return _moveDir; }
        set
        {
            if (_moveDir == value) return;

            if (State == CreatureState.Moving) return;

            _moveDir = value;
            if (value != MoveDirection.None)
                _lastDir = value;

            UpdateAnimation();
        }
    }

    public Vector3Int GetFrontCellPos()
    {
        Vector3Int cellPos = CellPos;

        switch (_lastDir)
        {
            case MoveDirection.Up:
                cellPos += Vector3Int.up;
                break;
            case MoveDirection.Down:
                cellPos += Vector3Int.down;
                break;
            case MoveDirection.Left:
                cellPos += Vector3Int.left;
                break;
            case MoveDirection.Right:
                cellPos += Vector3Int.right;
                break;
        }

        return cellPos;
    }

    protected virtual void UpdateAnimation()
    {
        //만약 State, Dir의 set이 호출되면 애니메이션 수정
        if (_state == CreatureState.Idle)
        {
            if (_moveDir != MoveDirection.None)
            {
                return;
            }

            switch (_lastDir)
            {
                case MoveDirection.Up:
                    _animator.Play("IDLE_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDirection.Down:
                    _animator.Play("IDLE_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDirection.Left:
                    _animator.Play("IDLE_RIGHT");
                    //The player looks the left, must be flipped horizontally.
                    _sprite.flipX = true;
                    break;
                case MoveDirection.Right:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else if (_state == CreatureState.Moving)
        {

            switch (_moveDir)
            {
                //Walking Animation
                case MoveDirection.Up:
                    _animator.Play("WALK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDirection.Down:
                    _animator.Play("WALK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDirection.Left:
                    _animator.Play("WALK_RIGHT");
                    //The player looks the left, must be flipped horizontally.
                    _sprite.flipX = true;
                    break;
                case MoveDirection.Right:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else if (_state == CreatureState.Skill)
        {
            //공격할 땐 _lastDir로 해야 한다.
            switch (_lastDir)
            {
                //Skill Animation
                case MoveDirection.Up:
                    _animator.Play("ATTACK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDirection.Down:
                    _animator.Play("ATTACK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDirection.Left:
                    _animator.Play("ATTACK_RIGHT");
                    //The player looks the left, must be flipped horizontally.
                    _sprite.flipX = true;
                    break;
                case MoveDirection.Right:
                    _animator.Play("ATTACK_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else if (_state == CreatureState.Dead)
        {
            //Maybe?
        }
    }

    protected virtual void Init()
    {
        _animator = gameObject.GetComponent<Animator>();
        _sprite = gameObject.GetComponent<SpriteRenderer>();
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.0f);
        transform.position = pos;
    }

    protected virtual void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Skill:
                UpdateSkill();
                break;
            case CreatureState.Dead:
                UpdateDead();
                break;
        }
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        UpdateController();
    }


    // Summary:
    //     이동 가능한 상태일 때(Idle일 때) 실제 좌표를 이동
    //     캐릭터 객체가 이동할 Grid 목표 위치 좌표
    protected virtual void UpdateIdle()
    {
        if (State == CreatureState.Idle && _moveDir != MoveDirection.None)
        {
            Vector3Int destPos = CellPos;
            switch (Dir)
            {
                case MoveDirection.Up:
                    destPos += Vector3Int.up;
                    break;
                case MoveDirection.Down:
                    destPos += Vector3Int.down;
                    break;
                case MoveDirection.Left:
                    destPos += Vector3Int.left;
                    break;
                case MoveDirection.Right:
                    destPos += Vector3Int.right;
                    break;
            }

            State = CreatureState.Moving;
            if (Managers.Map.CanMove(destPos))
            {
                if (Managers.Object.Find(destPos) == null)
                {
                    CellPos = destPos;
                }
            }
        }
    }


    //
    // Summary:
    //     캐릭터가 Grid상에서 한 칸 움직이게 하는 함수
    //     destPos : 캐릭터 스프라이트가 표시될 위치
    protected virtual void UpdateMoving()
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.0f);
        Vector3 moveDir = destPos - transform.position;

        float dist = moveDir.magnitude;

        if (dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;

            State = CreatureState.Idle;
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
            State = CreatureState.Moving;
        }

    }

    protected virtual void UpdateSkill() { }
    protected virtual void UpdateDead() { }
}



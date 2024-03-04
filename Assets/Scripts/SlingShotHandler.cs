using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlingShotHandler : MonoBehaviour
{
    [Header("Line Renderers")]
    [SerializeField] private LineRenderer _leftLineRenderer;
    [SerializeField] private LineRenderer _rightLineRenderer;
    [Header("Transform References")]
    [SerializeField] private Transform _leftStartPosition;
    [SerializeField] private Transform _rightStartPosition;

    [SerializeField] private Transform _centerPosition;
    [SerializeField] private Transform _idlePosition;
    [Header("Sling Shots Stats")]
    [SerializeField] private float _maxDistance = 3.5f;
    [SerializeField] private float _shotForce = 5f;
    [SerializeField] private float _timeBetweenBirdRespawn = 2f;

    [Header("Scripts")]
    [SerializeField] private SlingShotArea _slingShotArea;

    [Header("Bird")]
    [SerializeField] private AngryBird _birdPreFab;
    [SerializeField] private float _birdPositionOffset = 0.2f;

    private AngryBird _spawnedBird;

    private Vector2 _slingShotLinesPosition;
    private Vector2 _direction;
    private Vector2 _directionNormalized;

    private bool _clickedWithinSlingArea;

    private bool _birdOnSlingShot;

    void Awake()
    {
        _leftLineRenderer.enabled = false;
        _rightLineRenderer.enabled = false;

        SpawnBird();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && _slingShotArea.IsWithinSlingShotArea())
        {
            _clickedWithinSlingArea = true;
        }
        if (Mouse.current.leftButton.isPressed && _clickedWithinSlingArea && _birdOnSlingShot)
        {
            DrawSlingShot();
            PositionAndRotateBird();
        }
        if (Mouse.current.leftButton.wasReleasedThisFrame && _birdOnSlingShot)
        {
            if (GameManager.instance.HasEnoughShots())
            {
                _clickedWithinSlingArea = false;
                _spawnedBird.LaunchBird(_direction, _shotForce);
                GameManager.instance.UsedShots();
                _birdOnSlingShot = false;
                SetLines(_centerPosition.position);
                if (GameManager.instance.HasEnoughShots())
                {
                    StartCoroutine(SpwanBirdAfterTime());
                }
            }
        }
    }

    #region SlingShotMethods
    private void DrawSlingShot()
    {
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        _slingShotLinesPosition = _centerPosition.position + Vector3.ClampMagnitude(touchPosition - _centerPosition.position, _maxDistance);
        SetLines(_slingShotLinesPosition);

        _direction = (Vector2)_centerPosition.position - _slingShotLinesPosition;
        _directionNormalized = _direction.normalized;
    }

    private void SetLines(Vector2 position)
    {
        if (!_leftLineRenderer.enabled && !_rightLineRenderer.enabled)
        {
            _leftLineRenderer.enabled = true;
            _rightLineRenderer.enabled = true;
        }
        _leftLineRenderer.SetPosition(0, position);
        _leftLineRenderer.SetPosition(1, _leftStartPosition.position);

        _rightLineRenderer.SetPosition(0, position);
        _rightLineRenderer.SetPosition(1, _rightStartPosition.position);
    }
    #endregion

    #region Bird Methods
    private void SpawnBird()
    {
        SetLines(_idlePosition.position);
        Vector2 dir = (_centerPosition.position - _idlePosition.position).normalized;
        Vector2 spawnPosition = (Vector2)_idlePosition.position + dir * _birdPositionOffset;

        _spawnedBird = Instantiate(_birdPreFab, spawnPosition, Quaternion.identity);
        _spawnedBird.transform.right = dir;

        _birdOnSlingShot = true;
    }
    private void PositionAndRotateBird()
    {
        _spawnedBird.transform.position = _slingShotLinesPosition + _directionNormalized * _birdPositionOffset;
        _spawnedBird.transform.right = _directionNormalized;
    }

    private IEnumerator SpwanBirdAfterTime()
    {
        yield return new WaitForSeconds(_timeBetweenBirdRespawn);
        SpawnBird();
    }
    #endregion
}

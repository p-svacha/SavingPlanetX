using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraHandler
{
    private GameModel Model;
    private Camera Camera;

    private const float TURN_SPEED = 80f;
    private const float MOVE_SPEED = 10f;
    private const float ZOOM_SPEED = 50f;
    private const float OFFSET_RADIUS_SCALE = 1f; // How far the camera is from the center depending on the height 
    private const float MIN_HEIGHT = 2f;
    private const float MAX_HEIGHT = 20f;

    private const float CAMERA_MOVE_TIME = 3f;

    private const float CAMERA_SHAKE_INTERVAL = 0.05f;

    public CameraState State;

    // Camera Position
    private float Angle;
    private float OffsetRadius;
    private float CurrentHeight;
    private float MinHeight = 5f;
    private Vector2 Center;
    public float MaxHeight;
    private Vector2 CurrentPosition; // Camera is currently looking at this position

    // Camera movement
    private Vector2 SourcePosition; // Camera moves from this position
    private Vector2 TargetPosition; // Camera moves to this position
    private float CurrentMoveTime;

    // Camera shake
    private float ShakeIntensity;
    private float ShakeTime;
    private float CurrentShakeTime;
    private float CurrentShakeIntervalTime;
    private Vector2 ShakeSourcePosition;


    public CameraHandler(GameModel model)
    {
        Model = model;
        CurrentHeight = MinHeight;
        State = CameraState.Idle;
        Camera = Camera.main;
    }

    public void Update()
    {
        switch (State)
        {
            case CameraState.Idle:
                HandleInputs();
                break;

            case CameraState.Moving:
                CurrentMoveTime += Time.deltaTime;
                CurrentPosition = Vector3.Lerp(SourcePosition, TargetPosition, CurrentMoveTime / CAMERA_MOVE_TIME);
                if (CurrentMoveTime > CAMERA_MOVE_TIME)
                {
                    State = CameraState.Idle;
                    CurrentPosition = TargetPosition;
                }
                SetPosition();
                break;

            case CameraState.Shaking:
                CurrentShakeTime += Time.deltaTime;
                CurrentShakeIntervalTime += Time.deltaTime;
                if(CurrentShakeTime > ShakeTime)
                {
                    CurrentPosition = ShakeSourcePosition;
                    State = CameraState.Idle;
                }
                else
                {
                    CurrentPosition = Vector2.Lerp(SourcePosition, TargetPosition, CurrentShakeIntervalTime / CAMERA_SHAKE_INTERVAL);
                    if(CurrentShakeIntervalTime > CAMERA_SHAKE_INTERVAL)
                    {
                        SourcePosition = TargetPosition;
                        TargetPosition = GetShakeTargetPosition();
                        CurrentShakeIntervalTime = 0f;
                    }
                }
                SetPosition();
                break;
        }
    }

    #region Move Camera

    public void FocusVisibleTiles()
    {
        UpdateBounds();
        CurrentPosition = Center;
        SetPosition();
    }

    public void MoveTo(Tile t)
    {
        if (State != CameraState.Idle) return;
        State = CameraState.Moving;
        SourcePosition = CurrentPosition;
        TargetPosition = new Vector2(t.transform.position.x, t.transform.position.z);
        CurrentMoveTime = 0f;
    }

    #endregion

    #region Camera Shake

    public void Shake(float time, float intensity)
    {
        if (State != CameraState.Idle) return;
        ShakeIntensity = intensity;
        ShakeTime = time;
        CurrentShakeTime = 0f;
        CurrentShakeIntervalTime = 0f;
        SourcePosition = CurrentPosition;
        ShakeSourcePosition = CurrentPosition;
        TargetPosition = GetShakeTargetPosition();
        State = CameraState.Shaking;
    }

    private Vector2 GetShakeTargetPosition()
    {
        float angle = Random.Range(0, 360);
        float targetX = ShakeSourcePosition.x + ShakeIntensity * Mathf.Sin(Mathf.Deg2Rad * angle);
        float targetY = ShakeSourcePosition.y + ShakeIntensity * Mathf.Cos(Mathf.Deg2Rad * angle);
        return new Vector2(targetX, targetY);
    }

    #endregion

    public void UpdateBounds()
    {
        List<Tile> visibleTiles = Model.Map.VisibleTiles;
        float minX = visibleTiles.Min(x => x.transform.position.x);
        float maxX = visibleTiles.Max(x => x.transform.position.x);
        float minZ = visibleTiles.Min(x => x.transform.position.z);
        float maxZ = visibleTiles.Max(x => x.transform.position.z);

        Center.x = minX + (maxX - minX) / 2;
        Center.y = minZ + (maxZ - minZ) / 2;

        MaxHeight = 2 + Mathf.Max(maxX - minX, (maxZ - minZ) / 16 * 9) * 0.9f;
    }

    private void HandleInputs()
    {
        if (Input.GetKey(KeyCode.Q)) // Q - Rotate camera anti-clockwise
        {
            Angle = Angle += TURN_SPEED * Time.deltaTime;
            SetPosition();
        }
        if (Input.GetKey(KeyCode.E)) // E - Rotate camera clockwise
        {
            Angle = Angle -= TURN_SPEED * Time.deltaTime;
            SetPosition();
        }

        if (Input.GetKey(KeyCode.W)) // W - Move camera up
        {
            CurrentPosition.x -= MOVE_SPEED * Mathf.Sin(Mathf.Deg2Rad * Angle) * Time.deltaTime;
            CurrentPosition.y -= MOVE_SPEED * Mathf.Cos(Mathf.Deg2Rad * Angle) * Time.deltaTime;
            SetPosition();
        }
        if (Input.GetKey(KeyCode.A)) // A - Move camera left
        {
            CurrentPosition.x += MOVE_SPEED * Mathf.Sin(Mathf.Deg2Rad * (Angle + 90)) * Time.deltaTime;
            CurrentPosition.y += MOVE_SPEED * Mathf.Cos(Mathf.Deg2Rad * (Angle + 90)) * Time.deltaTime;
            SetPosition();
        }
        if (Input.GetKey(KeyCode.S)) // S - Move camera down
        {
            CurrentPosition.x += MOVE_SPEED * Mathf.Sin(Mathf.Deg2Rad * Angle) * Time.deltaTime;
            CurrentPosition.y += MOVE_SPEED * Mathf.Cos(Mathf.Deg2Rad * Angle) * Time.deltaTime;
            SetPosition();
        }
        if (Input.GetKey(KeyCode.D)) // D - Move camera right
        {
            CurrentPosition.x -= MOVE_SPEED * Mathf.Sin(Mathf.Deg2Rad * (Angle + 90)) * Time.deltaTime;
            CurrentPosition.y -= MOVE_SPEED * Mathf.Cos(Mathf.Deg2Rad * (Angle + 90)) * Time.deltaTime;
            SetPosition();
        }

        if (Input.mouseScrollDelta.y < 0) // Scroll down - Zoom out
        {
            CurrentHeight += ZOOM_SPEED * Time.deltaTime;
            SetPosition();
        }
        if (Input.mouseScrollDelta.y > 0) // Scroll up - Zoom in
        {
            CurrentHeight -= ZOOM_SPEED * Time.deltaTime;
            SetPosition();
        }
    }

    private void SetPosition()
    {
        CurrentHeight = Mathf.Clamp(CurrentHeight, MIN_HEIGHT, MAX_HEIGHT);

        OffsetRadius = OFFSET_RADIUS_SCALE * CurrentHeight;

        float cameraOffsetX = Mathf.Sin(Mathf.Deg2Rad * Angle) * OffsetRadius;
        float cameraOffsetY = Mathf.Cos(Mathf.Deg2Rad * Angle) * OffsetRadius;

        Camera.transform.position = new Vector3(CurrentPosition.x + cameraOffsetX, CurrentHeight, CurrentPosition.y + cameraOffsetY);
        Camera.transform.LookAt(new Vector3(CurrentPosition.x, 0, CurrentPosition.y));
    }
}

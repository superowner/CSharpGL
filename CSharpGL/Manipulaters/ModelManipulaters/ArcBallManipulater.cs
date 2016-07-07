﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;


namespace CSharpGL
{
    /// <summary>
    /// 用鼠标旋转模型。
    /// </summary>
    public class ArcBallManipulater
    {
        vec3 _vectorBack;
        vec3 _vectorUp;
        vec3 _vectorRight;
        float _length, _radiusRadius;
        CameraState cameraState = new CameraState();
        mat4 totalRotation = mat4.identity();
        vec3 _startPosition, _endPosition, _normalVector = new vec3(0, 1, 0);
        int _width;
        int _height;

        float mouseSensitivity = 0.1f;

        public float MouseSensitivity
        {
            get { return mouseSensitivity; }
            set { mouseSensitivity = value; }
        }

        /// <summary>
        /// 标识鼠标是否按下
        /// </summary>
        public bool MouseDownFlag { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ICamera Camera { get; private set; }

        /// <summary>
        /// 用鼠标旋转模型。
        /// </summary>
        /// <param name="camera">当前场景所用的摄像机。</param>
        public ArcBallManipulater(ICamera camera)
        {
            this.Camera = camera;

            SetCamera(camera.Position, camera.Target, camera.UpVector);
        }

        private void SetCamera(vec3 position, vec3 target, vec3 up)
        {
            _vectorBack = (position - target).normalize();
            _vectorUp = up;
            _vectorRight = _vectorUp.cross(_vectorBack).normalize();
            _vectorUp = _vectorBack.cross(_vectorRight).normalize();

            this.cameraState.position = position;
            this.cameraState.target = target;
            this.cameraState.up = up;
        }

        class CameraState
        {
            public vec3 position;
            public vec3 target;
            public vec3 up;

            public bool IsSameState(ICamera camera)
            {
                if (camera.Position != this.position) { return false; }
                if (camera.Target != this.target) { return false; }
                if (camera.UpVector != this.up) { return false; }

                return true;
            }
        }

        public void SetBounds(int width, int height)
        {
            this._width = width; this._height = height;
            _length = width > height ? width : height;
            var rx = (width / 2) / _length;
            var ry = (height / 2) / _length;
            _radiusRadius = (float)(rx * rx + ry * ry);
        }

        /// <summary>
        /// 必须先调用<see cref="SetBounds"/>()方法。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MouseDown(int x, int y)
        {
            if (!cameraState.IsSameState(this.Camera))
            {
                SetCamera(this.Camera.Position, this.Camera.Target, this.Camera.UpVector);
            }

            this._startPosition = GetArcBallPosition(x, y);

            MouseDownFlag = true;
        }

        private vec3 GetArcBallPosition(int x, int y)
        {
            float rx = (x - _width / 2) / _length;
            float ry = (_height / 2 - y) / _length;
            float zz = _radiusRadius - rx * rx - ry * ry;
            float rz = (zz > 0 ? (float)Math.Sqrt(zz) : 0.0f);
            var result = new vec3(
                rx * _vectorRight.x + ry * _vectorUp.x + rz * _vectorBack.x,
                rx * _vectorRight.y + ry * _vectorUp.y + rz * _vectorBack.y,
                rx * _vectorRight.z + ry * _vectorUp.z + rz * _vectorBack.z
                );
            //var radius = new vec3(rx, ry, rz);
            //var matrix = new mat3(_vectorRight, _vectorUp, _vectorBack);
            //result = matrix * new vec3(rx, ry, rz);

            return result;
        }


        public void MouseMove(int x, int y)
        {
            if (MouseDownFlag)
            {
                if (!cameraState.IsSameState(this.Camera))
                {
                    SetCamera(this.Camera.Position, this.Camera.Target, this.Camera.UpVector);
                }

                this._endPosition = GetArcBallPosition(x, y);
                var cosAngle = _startPosition.dot(_endPosition) / (_startPosition.length() * _endPosition.length());
                if (cosAngle > 1) { cosAngle = 1; }
                else if (cosAngle < -1) { cosAngle = -1; }
                var angle = mouseSensitivity * (float)(Math.Acos(cosAngle) / Math.PI * 180);
                _normalVector = _startPosition.cross(_endPosition).normalize();
                if (!
                    ((_normalVector.x == 0 && _normalVector.y == 0 && _normalVector.z == 0)
                    || float.IsNaN(_normalVector.x) || float.IsNaN(_normalVector.y) || float.IsNaN(_normalVector.z)))
                {
                    _startPosition = _endPosition;

                    mat4 newRotation = glm.rotate(angle, _normalVector);
                    this.totalRotation = newRotation * totalRotation;
                }
            }
        }

        public void MouseUp(int x, int y)
        {
            MouseDownFlag = false;
        }

        public mat4 GetRotationMatrix()
        {
            return totalRotation;
        }
    }
}

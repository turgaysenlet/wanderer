﻿using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wanderer
{
    public class Objects : IDisposable
    {
        public Objects(ShaderType shaderType, string[] source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            // Create
            ShaderName = Gl.CreateShader(shaderType);
            // Submit source code
            Gl.ShaderSource(ShaderName, source);
            // Compile
            Gl.CompileShader(ShaderName);
            // Check compilation status
            int compiled;

            Gl.GetShader(ShaderName, ShaderParameterName.CompileStatus, out compiled);
            if (compiled != 0)
                return;

            // Throw exception on compilation errors
            const int logMaxLength = 1024;

            StringBuilder infolog = new StringBuilder(logMaxLength);
            int infologLength;

            Gl.GetShaderInfoLog(ShaderName, logMaxLength, out infologLength, infolog);

            throw new InvalidOperationException($"unable to compile shader: {infolog}");
        }

        public readonly uint ShaderName;

        public void Dispose()
        {
            Gl.DeleteShader(ShaderName);
        }
    }
}

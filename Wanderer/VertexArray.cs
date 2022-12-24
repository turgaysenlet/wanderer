using OpenGL;

namespace Wanderer
{
    public class VertexArray : IDisposable
    {
        public VertexArray(Programs program, float[] positions, float[] colors)
        {
            if (program == null)
                throw new ArgumentNullException(nameof(program));

            // Allocate buffers referenced by this vertex array
            _BufferPosition = new Wanderer.Buffer(positions);
            _BufferColor = new Wanderer.Buffer(colors);

            // Generate VAO name
            ArrayName = Gl.GenVertexArray();
            // First bind create the VAO
            Gl.BindVertexArray(ArrayName);

            // Set position attribute

            // Select the buffer object
            Gl.BindBuffer(BufferTarget.ArrayBuffer, _BufferPosition.BufferName);
            // Format the vertex information: 2 floats from the current buffer
            Gl.VertexAttribPointer((uint)program.LocationPosition, 3, VertexAttribType.Float, false, 0, IntPtr.Zero);
            // Enable attribute
            Gl.EnableVertexAttribArray((uint)program.LocationPosition);

            // As above, but for color attribute
            Gl.BindBuffer(BufferTarget.ArrayBuffer, _BufferColor.BufferName);
            Gl.VertexAttribPointer((uint)program.LocationColor, 3, VertexAttribType.Float, false, 0, IntPtr.Zero);
            Gl.EnableVertexAttribArray((uint)program.LocationColor);
        }

        public readonly uint ArrayName;

        private readonly Wanderer.Buffer _BufferPosition;

        private readonly Wanderer.Buffer _BufferColor;

        public void Dispose()
        {
            Gl.DeleteVertexArrays(ArrayName);

            _BufferPosition.Dispose();
            _BufferColor.Dispose();
        }
    }

}

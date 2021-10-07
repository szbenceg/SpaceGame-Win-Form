
namespace SpaceGame.Model
{
    class Target : ComponentProperties
    {
        public string status = "";
        public Target(int psX, int psY, int width, int height) {
            PositionX = psX;
            PositionY = psY;
            Speed = 10;
            Width = width;
            Height = height;
        }

    }
}

namespace VerseVox.Scripts
{
    public static class NonStaticVariables
    {
        //Main Variables
        public static bool successLogin = false;
        public static bool successRegister = false;
        public static int windowId = 1000;
        public static string errorMessage = "";

        //RightSide Variables
        public static int ChatID;
        public static int leftHalfWindowID = 0;

        //LeftSide Variables
        public static int downWindowID = 0;
        public static int middleWindowID = 0;
        public static int HalfWindowID = 0;
        public static int upWindowID = 0;

        //Dialog Menu Variables
        public static int selectedUserId = 0;


        public static bool hiddenWindow = false;
        public static bool isCalling = false;
        public static bool isConnected = false;
        public static int clientChannel = 0;
    }
}

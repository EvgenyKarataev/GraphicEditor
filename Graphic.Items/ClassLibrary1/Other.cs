using System;

namespace Graphic.Items
{
    public enum Nows { None, Line, Rec, Ellip, Pie, Group};
    public enum SaveMode { sSavePage, sSaveAlbum, sSaveAlbumAs};
    public enum OpenMode { oOpen, oAddFromFile, oAddFromAlbum};
    public enum Corners { None, TopLeft, Left, BottomLeft, Bottom, BottomRight, Right, TopRight, Top};

    public struct SavedResult
    {
        public bool Modificated;
        public string Name;
    }

    public struct ItemOfRecycle
    {
        public Object Item;
        public string Name;
    }
}

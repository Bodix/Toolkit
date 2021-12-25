// Copyright © 2020 Bogdan Nikolayev <bodix321@gmail.com>
// All Rights Reserved

using System.IO;

public static class PathUtility
{
    public static string ChangeFileName(string path, string fileName)
    {
        return path.Replace(Path.GetFileName(path), fileName);
    }

    public static string ChangeFileNameWithoutExtension(string path, string fileNameWithoutExtension)
    {
        return path.Replace(Path.GetFileNameWithoutExtension(path), fileNameWithoutExtension);
    }
}
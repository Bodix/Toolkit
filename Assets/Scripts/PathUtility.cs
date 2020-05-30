/*
 * Copyright (C) 2020 by Evolutex - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Bogdan Nikolaev <bodix321@gmail.com>
*/

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
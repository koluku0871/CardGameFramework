#if UNITY_EDITOR
using System.IO;
using System.IO.Compression;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// ビルド完了時にZipファイルを作成できるEditor拡張
/// </summary>
public class PostprocessBuildZip : IPostprocessBuildWithReport
{
    // ビルド完了コールバックの優先順位
    public int callbackOrder { get { return 0; } }

    /// <summary>
    /// ビルド完了時にZipファイルを作成する
    /// </summary>
    /// <param name="report"></param>
    public void OnPostprocessBuild(BuildReport report)
    {
        // プロジェクト名の取得
        var directoryInfo = new DirectoryInfo(Application.dataPath).Parent;
        string projectName = directoryInfo!.Name;


        // 現在の日時をフォーマット
        string dateTimeFormat = report.summary.buildEndedAt.ToString("yyMMddHHmmss");

        // "Build"サブディレクトリのパス
        string buildDirectory = Path.Combine("./", "Build");

        // ディレクトリが存在しない場合は作成
        if (!Directory.Exists(buildDirectory))
        {
            Directory.CreateDirectory(buildDirectory);
        }

        // Zipファイルの生成
        string userSettingBuildPath = System.IO.Path.GetDirectoryName(report.summary.outputPath); // ユーザーが指定したビルド先のパス
        string zipFileName = $"{projectName}_{dateTimeFormat}.zip"; // Zipファイル名
        string zipPath = Path.Combine(buildDirectory, zipFileName);
        ZipFile.CreateFromDirectory(userSettingBuildPath, zipPath);
        Debug.Log("Build zip created at: " + zipPath);
    }
}
#endif
#if UNITY_EDITOR
using System.IO;
using System.IO.Compression;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// �r���h��������Zip�t�@�C�����쐬�ł���Editor�g��
/// </summary>
public class PostprocessBuildZip : IPostprocessBuildWithReport
{
    // �r���h�����R�[���o�b�N�̗D�揇��
    public int callbackOrder { get { return 0; } }

    /// <summary>
    /// �r���h��������Zip�t�@�C�����쐬����
    /// </summary>
    /// <param name="report"></param>
    public void OnPostprocessBuild(BuildReport report)
    {
        // �v���W�F�N�g���̎擾
        var directoryInfo = new DirectoryInfo(Application.dataPath).Parent;
        string projectName = directoryInfo!.Name;


        // ���݂̓������t�H�[�}�b�g
        string dateTimeFormat = report.summary.buildEndedAt.ToString("yyMMddHHmmss");

        // "Build"�T�u�f�B���N�g���̃p�X
        string buildDirectory = Path.Combine("./", "Build");

        // �f�B���N�g�������݂��Ȃ��ꍇ�͍쐬
        if (!Directory.Exists(buildDirectory))
        {
            Directory.CreateDirectory(buildDirectory);
        }

        // Zip�t�@�C���̐���
        string userSettingBuildPath = System.IO.Path.GetDirectoryName(report.summary.outputPath); // ���[�U�[���w�肵���r���h��̃p�X
        string zipFileName = $"{projectName}_{dateTimeFormat}.zip"; // Zip�t�@�C����
        string zipPath = Path.Combine(buildDirectory, zipFileName);
        ZipFile.CreateFromDirectory(userSettingBuildPath, zipPath);
        Debug.Log("Build zip created at: " + zipPath);
    }
}
#endif
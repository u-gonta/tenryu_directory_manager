﻿using System;
using System.IO;
using System.Windows;

namespace DirectoryManagerTsp
{
    namespace Generation
    {
        namespace Equipment
        {
            /// <summary>
            /// WindowRevisionUp.xaml の相互作用ロジック
            /// </summary>
            public partial class WindowRevisionUp : Window
            {
                /// <summary>
                /// 機種名
                /// </summary>
                private string _model = Shared.Select.ClassDefine.UnselectedName;

                /// <summary>
                /// 参照する装置名
                /// </summary>
                private string _equipmentSource = Shared.Select.ClassDefine.UnselectedName;

                /// <summary>
                /// リビジョンアップ後の装置名
                /// </summary>
                private string _equipmentDestination = Shared.Select.ClassDefine.UnselectedName;

                /// <summary>
                /// コンストラクタ
                /// </summary>
                /// <param name="model">機種名</param>
                /// <param name="equipment">装置名</param>
                public WindowRevisionUp(string model, string equipment)
                {
                    InitializeComponent();

                    try
                    {
                        // 機種名を更新
                        _model = model;

                        // 参照する装置名を更新
                        _equipmentSource = equipment;

                        // リビジョンアップ後の装置名を更新
                        _equipmentDestination = Tool.ClassEquipment.Rename(equipment, Tool.ClassEquipment.Revision(equipment) + 1);

                        // 画面を初期化
                        InitializeWindow();
                    }
                    catch (Exception ex)
                    {
                        // エラーのメッセージを表示
                        Shared.Tool.ClassException.Message(ex);
                    }
                }

                /// <summary>
                /// 作成ボタンクリック
                /// </summary>
                /// <param name="sender"></param>
                /// <param name="e"></param>
                private void buttonGeneration_Click(object sender, RoutedEventArgs e)
                {
                    try
                    {
                        var settingManager = Setting.ClassIntegrationManager.Instance;
                        var integration = settingManager.GetIntegration();
                        var equipment = new ClassEquipment();

                        // 機種名を更新
                        equipment.Model = _model;

                        // 装置名を更新
                        equipment.Name = _equipmentDestination;

                        // 部署を走査
                        foreach (var department in integration.Departments)
                        {
                            // 部署のディレクトリ内に機種と装置のディレクトリを作成
                            Directory.CreateDirectory(Shared.Tool.ClassPath.DirectoryDelimiter(department.RootDirectory) + Shared.Tool.ClassPath.DirectoryDelimiter(equipment.Model) + equipment.Name);

                            // リビジョンアップ
                            ClassManager.RevisionUp(department.RootDirectory, equipment);
                        }

                        // リビジョンアップ後の装置ディレクトリのパスを取得
                        var path = GetDestinationPath();

                        // ディレクトリを作成
                        Directory.CreateDirectory(path);

                        // 参照する装置ディレクトリにある情報のファイル読み込み
                        var information = ClassManager.LoadDirectory(path);

                        // 派生元の機種名を付加
                        information.DerivationModel = _model;

                        // 派生元の装置名を付加
                        information.DerivationEquipment = _equipmentSource;

                        // コメントを付加
                        information.Comment = textBoxComment.Text;

                        // 情報のファイルを書き込み
                        ClassManager.SaveDirectory(information, path);

                        // 戻り値を更新
                        this.DialogResult = true;

                        // 画面を閉じる
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        // エラーのメッセージを表示
                        Shared.Tool.ClassException.Message(ex);
                    }
                }

                /// <summary>
                /// 画面を初期化
                /// </summary>
                private void InitializeWindow()
                {
                    // 機種名を更新
                    labelModel.Content = _model;

                    // 参照する装置名を更新
                    labelEquipmentSource.Content = _equipmentSource;

                    // リビジョンアップ後の装置名を更新
                    labelEquipmentDestination.Content = _equipmentDestination;

                    // 機種ディレクトリのパスを取得
                    var path = GetModelPath();

                    // 参照する機種ディレクトリにある情報のファイル読み込み
                    var information = ClassManager.LoadDirectory(path);

                    // 機種のコメントを更新
                    labelModelComment.Content = information.Comment;

                    // 参照する装置ディレクトリを取得
                    path = GetSourcePath();

                    // 参照する装置ディレクトリにある情報のファイル読み込み
                    information = ClassManager.LoadDirectory(path);

                    // 派生元の装置名を確認
                    if (0 < information.DerivationEquipment.Length)
                    {
                        // 派生元の装置名がある ⇒ 派生元の装置名を付加
                        labelEquipmentSourceComment.Content = "派生元：" + information.DerivationEquipment;
                        labelEquipmentSourceComment.Content += "\n";
                    }

                    // コメントを更新
                    labelEquipmentSourceComment.Content += information.Comment;
                    textBoxComment.Text = information.Comment;
                }

                /// <summary>
                /// 機種ディレクトリを取得
                /// </summary>
                /// <returns>機種ディレクトリのパス</returns>
                private string GetModelPath()
                {
                    string ret = "";

                    var settingManager = Setting.ClassIntegrationManager.Instance;
                    var integration = settingManager.GetIntegration();

                    // テンプレートのルートディレクトリ
                    ret = integration.TemplateRootDirectory;

                    // ディレクトリの区切りを付加
                    ret = Shared.Tool.ClassPath.DirectoryDelimiter(ret);

                    // 機種名を付加
                    ret += _model;

                    return ret;
                }

                /// <summary>
                /// 参照する装置ディレクトリを取得
                /// </summary>
                /// <returns>参照する装置ディレクトリ</returns>
                private string GetSourcePath()
                {
                    string ret = "";

                    // 機種ディレクトリを取得
                    ret = GetModelPath();

                    // ディレクトリの区切りを付加
                    ret = Shared.Tool.ClassPath.DirectoryDelimiter(ret);

                    // 装置名を付加
                    ret += _equipmentSource;

                    return ret;
                }

                /// <summary>
                /// リビジョンアップ後の装置ディレクトリを取得
                /// </summary>
                /// <returns>リビジョンアップ後の装置ディレクトリ</returns>
                private string GetDestinationPath()
                {
                    string ret = "";

                    // 機種ディレクトリを取得
                    ret = GetModelPath();

                    // ディレクトリの区切りを付加
                    ret = Shared.Tool.ClassPath.DirectoryDelimiter(ret);

                    // 装置名を付加
                    ret += _equipmentDestination;

                    return ret;
                }
            }
        }
    }
}
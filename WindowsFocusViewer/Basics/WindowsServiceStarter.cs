using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFocusViewer
{
    public class WindowsServiceStarter
    {
        private readonly string _ServicePath;

        public string ServiceName { get; }

        public WindowsServiceStarter(string servicePath,string serviceName)
        {
            ServicePathCheck(servicePath);
            _ServicePath = servicePath;
            ServiceName = serviceName;
        }

        public async Task<bool> Start()
        {
            var service = new ServiceController(ServiceName);
            var result = await Task.Run(() =>
            {
                using (service)
                {
                    try
                    {
                        var timeout = TimeSpan.FromSeconds(3);
                        switch (service.Status)
                        {
                            case ServiceControllerStatus.ContinuePending:
                                break;
                            case ServiceControllerStatus.Paused:
                                service.Continue();
                                break;
                            case ServiceControllerStatus.PausePending:
                                service.WaitForStatus(ServiceControllerStatus.Paused, timeout);
                                service.Continue();
                                break;
                            case ServiceControllerStatus.Running:
                                break;
                            case ServiceControllerStatus.StartPending:
                                break;
                            case ServiceControllerStatus.Stopped:
                                service.Start();
                                break;
                            case ServiceControllerStatus.StopPending:
                                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                                service.Start();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Console.WriteLine(ex);
#endif
                        return false;
                    }
                }
                return true;
            });
            return result;
        }

        public async Task<bool> InstallService()
        {
            using (var installer = new AssemblyInstaller())
            {
                installer.UseNewContext = true;
                installer.Path = _ServicePath;
                try
                {
                    var state = new Hashtable();
                    await Task.Run(() =>
                    {
                        installer.Install(state);
                        installer.Commit(state);
                    });
                    return true;
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine(ex);
#endif
                    return false;
                }
            }
        }

        public bool HasService()
        {
            try
            {
                var service = new ServiceController(ServiceName);
                using (service)
                {
                    // 读取一次服务状态，判定是否已安装服务
                    // 如果，抛出异常 Win32Exception 表示未安装
                    var status = service.Status;
                }
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            return true;
        }

        public ServiceControllerStatus GetStatus()
        {
            var service = new ServiceController(ServiceName);
            using (service)
            {
                return service.Status;
            }
        }

        private void ServicePathCheck(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path),"Windows 服务路径不能为空");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("未找到 Windows 服务应用程序文件");
            }
        }   
    }

    public static class TaskExt
    {
        public static async void Loading(this ILoadHandler handler,Task task)
        {
            try
            {
                handler.IsLoading = true;
                await task;
            }
            finally
            {
                handler.IsLoading = false;
            }
        }

    }

    public interface ILoadHandler
    {
        bool IsLoading { get; set; }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

public class HttpServerManager : MonoBehaviour
{
    public static HttpListener httpListener;
    // Start is called before the first frame update
    void Start()
    {
        httpListener = new HttpListener();

        httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        httpListener.Prefixes.Add("http://+:443/");
       // httpListener.Prefixes.Add("https://+:443/");

        httpListener.Start();

        httpListener.BeginGetContext(Result, null);

        

    }
    public static void Result(IAsyncResult ar) {
        //当接收到请求后程序流会走到这里

        //继续异步监听
        httpListener.BeginGetContext(Result, null);
        var guid = Guid.NewGuid().ToString();
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"接到新的请求:{guid},时间：{DateTime.Now.ToString()}");
        //获得context对象

        var returnByteArr = File.ReadAllBytes("D:\\orig.zip");//Encoding.UTF8.GetBytes(returnObj);//设置客户端返回信息的编码
        var size = returnByteArr.Length;

        var context = httpListener.EndGetContext(ar);
        var request = context.Request;
        var response = context.Response;
        context.Response.ContentType = "application/zip";//告诉客户端返回的ContentType类型为纯文本格式，编码为UTF-8
        context.Response.AddHeader("Content-type", "application/zip");//添加响应头信息
        context.Response.AddHeader("Content-Length", ""+ size);//添加响应头信息
        context.Response.AddHeader("Accept-Ranges", "bytes");//添加响应头信息
        context.Response.AddHeader("Content-Disposition","attachment; filename = test11zip.zip");
        string returnObj = null;//定义返回客户端的信息
        if (request.HttpMethod == "POST" && request.InputStream != null)
        {
            //处理客户端发送的请求并返回处理信息
            returnObj = "123";
        }
        else
        {
            returnObj = $"不是post请求或者传过来的数据为空";
        }

        try
        {
            using (var stream = response.OutputStream)
            {
                //把处理信息返回到客户端
                stream.Write(returnByteArr, 0, returnByteArr.Length);
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"网络蹦了：{ex.ToString()}");
        }
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"请求处理完成：{guid},时间：{ DateTime.Now.ToString()}\r\n");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


/*
public static void Result(IAsyncResult ar)
{
    //当接收到请求后程序流会走到这里

    //继续异步监听
    httpListener.BeginGetContext(Result, null);
    var guid = Guid.NewGuid().ToString();
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine($"接到新的请求:{guid},时间：{DateTime.Now.ToString()}");
    //获得context对象
    var context = httpListener.EndGetContext(ar);
    var request = context.Request;
    var response = context.Response;
    ////如果是js的ajax请求，还可以设置跨域的ip地址与参数
    //context.Response.AppendHeader("Access-Control-Allow-Origin", "*");//后台跨域请求，通常设置为配置文件
    //context.Response.AppendHeader("Access-Control-Allow-Headers", "ID,PW");//后台跨域参数设置，通常设置为配置文件
    //context.Response.AppendHeader("Access-Control-Allow-Method", "post");//后台跨域请求设置，通常设置为配置文件
    context.Response.ContentType = "text/plain;charset=UTF-8";//告诉客户端返回的ContentType类型为纯文本格式，编码为UTF-8
    context.Response.AddHeader("Content-type", "text/plain");//添加响应头信息
    context.Response.ContentEncoding = Encoding.UTF8;
    string returnObj = null;//定义返回客户端的信息
    if (request.HttpMethod == "POST" && request.InputStream != null)
    {
        //处理客户端发送的请求并返回处理信息
        returnObj = "123";
    }
    else
    {
        returnObj = $"不是post请求或者传过来的数据为空";
    }
    var returnByteArr = Encoding.UTF8.GetBytes(returnObj);//设置客户端返回信息的编码
    try
    {
        using (var stream = response.OutputStream)
        {
            //把处理信息返回到客户端
            stream.Write(returnByteArr, 0, returnByteArr.Length);
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"网络蹦了：{ex.ToString()}");
    }
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"请求处理完成：{guid},时间：{ DateTime.Now.ToString()}\r\n");
}*/

/*
 new Thread(new ThreadStart(delegate
 {
     while (true)
     {

         HttpListenerContext httpListenerContext = httpListener.GetContext();
         httpListenerContext.Response.StatusCode = 200;

         using (StreamWriter writer = new StreamWriter(httpListenerContext.Response.OutputStream))
         {
             //writer.wr
             writer.WriteLine("<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/><title>测试服务器</title></head><body>");
             writer.WriteLine("<div style=\"height:20px;color:blue;text-align:center;\"><p> hello</p></div>");
             writer.WriteLine("<ul>");
             writer.WriteLine("</ul>");
             writer.WriteLine("</body></html>");

         }

     }
 })).Start();*/

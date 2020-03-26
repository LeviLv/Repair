using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using Newtonsoft.Json;
using System;

namespace Repair.SMS
{
    public static class SmsHelper
    {
        public static void SendAcs(string mobile, dynamic json)
        {

            if (string.IsNullOrEmpty(mobile))
            {
                throw new Exception("mobile不能为空");
            }
            string product = "Dysmsapi";//短信API产品名称（短信产品名固定，无需修改）
            string domain = "dysmsapi.aliyuncs.com";//短信API产品域名（接口地址固定，无需修改）
            string accessKeyId = "LTAI4FfhUHPcQ1VHcKN5wEHc";//你的accessKeyId，参考本文档步骤2
            string accessKeySecret = "F2zwYrZ4xKUwayfhXWXmthzLKiMXnp";//你的accessKeySecret，参考本文档步骤2

            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);
            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            var request = new SendSmsRequest();

            try
            {
                request.PhoneNumbers = mobile;
                //必填:短信签名-可在短信控制台中找到
                request.SignName = "Chak小修";
                //必填:短信模板-可在短信控制台中找到，发送国际/港澳台消息时，请使用国际/港澳台短信模版
                request.TemplateCode = "SMS_184211856";
                //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
                request.TemplateParam = JsonConvert.SerializeObject(json);// "{\"name\":\"Tom\",\"code\":\"123\"}";
                //请求失败这里会抛ClientException异常
                var sendSmsResponse = acsClient.GetAcsResponse(request);
                if (sendSmsResponse.BizId == null)
                    throw new ApplicationException(sendSmsResponse.Message);
                Console.WriteLine(sendSmsResponse.Message);
            }
            catch (ServerException ex)
            {
                throw new ApplicationException(ex.Message);
            }
            catch (ClientException ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public static void sendUserMsg(string mobile, dynamic json)
        {
            if (string.IsNullOrEmpty(mobile))
            {
                throw new Exception("mobile不能为空");
            }
            string product = "Dysmsapi";//短信API产品名称（短信产品名固定，无需修改）
            string domain = "dysmsapi.aliyuncs.com";//短信API产品域名（接口地址固定，无需修改）
            string accessKeyId = "LTAI4FfhUHPcQ1VHcKN5wEHc";//你的accessKeyId，参考本文档步骤2
            string accessKeySecret = "F2zwYrZ4xKUwayfhXWXmthzLKiMXnp";//你的accessKeySecret，参考本文档步骤2

            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);
            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            var request = new SendSmsRequest();

            try
            {
                request.PhoneNumbers = mobile;
                //必填:短信签名-可在短信控制台中找到
                request.SignName = "物业智能维修";
                //必填:短信模板-可在短信控制台中找到，发送国际/港澳台消息时，请使用国际/港澳台短信模版
                request.TemplateCode = "SMS_186599365";
                //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
                request.TemplateParam = JsonConvert.SerializeObject(json);// "{\"name\":\"Tom\",\"code\":\"123\"}";
                //请求失败这里会抛ClientException异常
                var sendSmsResponse = acsClient.GetAcsResponse(request);
                if (sendSmsResponse.BizId == null)
                    throw new ApplicationException(sendSmsResponse.Message);
                Console.WriteLine(sendSmsResponse.Message);
            }
            catch (ServerException ex)
            {
                throw new ApplicationException(ex.Message);
            }
            catch (ClientException ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public static void sendRepairMsg(string mobile, dynamic json)
        {
            if (string.IsNullOrEmpty(mobile))
            {
                throw new Exception("mobile不能为空");
            }
            string product = "Dysmsapi";//短信API产品名称（短信产品名固定，无需修改）
            string domain = "dysmsapi.aliyuncs.com";//短信API产品域名（接口地址固定，无需修改）
            string accessKeyId = "LTAI4FfhUHPcQ1VHcKN5wEHc";//你的accessKeyId，参考本文档步骤2
            string accessKeySecret = "F2zwYrZ4xKUwayfhXWXmthzLKiMXnp";//你的accessKeySecret，参考本文档步骤2

            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);
            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            var request = new SendSmsRequest();

            try
            {
                request.PhoneNumbers = mobile;
                //必填:短信签名-可在短信控制台中找到
                request.SignName = "物业智能维修";
                //必填:短信模板-可在短信控制台中找到，发送国际/港澳台消息时，请使用国际/港澳台短信模版
                request.TemplateCode = "SMS_186579241";
                //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
                request.TemplateParam = JsonConvert.SerializeObject(json);// "{\"name\":\"Tom\",\"code\":\"123\"}";
                //请求失败这里会抛ClientException异常
                var sendSmsResponse = acsClient.GetAcsResponse(request);
                if (sendSmsResponse.BizId == null)
                    throw new ApplicationException(sendSmsResponse.Message);
                Console.WriteLine(sendSmsResponse.Message);
            }
            catch (ServerException ex)
            {
                throw new ApplicationException(ex.Message);
            }
            catch (ClientException ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public static void sendAdminMsg(string mobile, dynamic json)
        {
            if (string.IsNullOrEmpty(mobile))
            {
                throw new Exception("mobile不能为空");
            }
            string product = "Dysmsapi";//短信API产品名称（短信产品名固定，无需修改）
            string domain = "dysmsapi.aliyuncs.com";//短信API产品域名（接口地址固定，无需修改）
            string accessKeyId = "LTAI4FfhUHPcQ1VHcKN5wEHc";//你的accessKeyId，参考本文档步骤2
            string accessKeySecret = "F2zwYrZ4xKUwayfhXWXmthzLKiMXnp";//你的accessKeySecret，参考本文档步骤2

            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);
            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            var request = new SendSmsRequest();

            try
            {
                request.PhoneNumbers = mobile;
                //必填:短信签名-可在短信控制台中找到
                request.SignName = "物业智能维修";
                //必填:短信模板-可在短信控制台中找到，发送国际/港澳台消息时，请使用国际/港澳台短信模版
                request.TemplateCode = "SMS_186599351";
                //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
                request.TemplateParam = JsonConvert.SerializeObject(json);// "{\"name\":\"Tom\",\"code\":\"123\"}";
                //请求失败这里会抛ClientException异常
                var sendSmsResponse = acsClient.GetAcsResponse(request);
                if (sendSmsResponse.BizId == null)
                    throw new ApplicationException(sendSmsResponse.Message);
                Console.WriteLine(sendSmsResponse.Message);
            }
            catch (ServerException ex)
            {
                throw new ApplicationException(ex.Message);
            }
            catch (ClientException ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Models.Utils
{
    public enum UserStatusEnums
    {
        [Description("Normal")]
        已通过 = 0,

        [Description("Check pending")]
        待审核 = 1,

        [Description("Locked")]
        已锁定 = 2,

    }

    public enum MaterialStatusEnums
    {
        [Description("Editing")]
        编辑中 = 0,

        [Description("On line")]
        已上架 = 1,

        [Description("Off line")]
        已下架 = 2,

    }

    public enum RoleEnums
    {
        [Description("Super administrator")]
        超级管理员 = 1,

        [Description("Administrator")]
        管理员 = 2,

        [Description("Adviser")]
        顾问 = 3,

        [Description("Client")]
        客户 = 4,
    }


    public enum PhoneTypeEnums
    {
        [Description("+86")]
        中国 = 1,

        [Description("+61")]
        澳洲 = 2
    }

    public enum SysMessageEnums
    {
        用户注册 = 0
    }

    public enum UploadTypeEnums
    {
        管理员 = 1,
        经销商 = 2,
    }


    public enum SendMessageEnums
    {
        发送成功 = 0,
        发送失败 = 1,
        无此用户 = 101,
        密码错 = 102,
        提交过快 = 103,
        系统忙 = 104,
        敏感短信 = 105,
        消息长度错 = 106,
        包含错误的手机号码 = 107,
        手机号码个数错 = 108,
        无发送额度 = 109,
        不在发送时间内 = 110,
        超出该账户当月发送额度限制 = 111,
        无此产品 = 112,
        extno格式错 = 113,
        签名不合法 = 116,
        用户没有相应的发送权限 = 118,
        用户已过期 = 119,
        内容不在白名单模板中 = 120
    }

    /// <summary>
    /// 产品多媒体类型
    /// </summary>
    public enum MediaTypeEnums
    {
        [Description("Video")]
        视频 = 1,

        [Description("Audio")]
        音频 = 2,

        [Description("Picture")]
        图片 = 3,

        [Description("Document")]
        文档 = 4,
    }


    public enum UploadImageEnums
    {
        身份证 = 1,
        用户头像 = 2,
        产品视频
    }

    public enum MaterialTypeEnums
    {
        [Description("Investment report")]
        投资报告 = 1,

        [Description("Statement")]
        声明 = 2
    }

}

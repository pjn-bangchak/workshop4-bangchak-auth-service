using System.ComponentModel.DataAnnotations;

namespace BangchakAuthService.ModelsDto;

public class RegisterDto {

    [Required(ErrorMessage = "ชื่อสกุล ห้ามว่าง")]
    public string Fullname {get; set;} = null!;

    [Required(ErrorMessage = "อีเมล์ ห้ามว่าง")]
    [EmailAddress(ErrorMessage = "รูปแบบอีเมล์ไม่ถูกต้อง")]
    public string Email {get; set;} = null!;

    [Required(ErrorMessage = "รหัสผ่าน ห้ามว่าง")]
    public string Password {get; set;} = null!;
}


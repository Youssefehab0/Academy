using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academy.Migrations
{
    /// <inheritdoc />
    public partial class Hatem2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAKy0xPjDhdrDmut/jX/kIjSNfAfSIrjv5ZfmhAHjybM73Umk7rs9NM1AclMtH5Vpw==");

            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "Id", "Email", "FullName", "PasswordHash", "RefreshToken", "RefreshTokenExpiry" },
                values: new object[] { 2, "hatemmedhat247@gmail.com", "Hatem Medhat", "AQAAAAIAAYagAAAAEAhFOxrcwN7HuqhIDKD+Wb1V0KHAiHsZhB3DTI8of4Xej7eTsoEUp22vT930lYk3lQ==", null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEM236fR8tXnyDAqqws3NivL0OoAHOZkE703wqZEs0D8maZMwlKiv2wPghCwFeaOSdw==");
        }
    }
}

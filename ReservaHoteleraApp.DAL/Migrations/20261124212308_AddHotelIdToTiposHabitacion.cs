using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservaHoteleraApp.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelIdToTiposHabitacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TiposHabitacion_Nombre",
                table: "TiposHabitacion");

            migrationBuilder.AddColumn<int>(
                name: "IdHotel",
                table: "TiposHabitacion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Los tipos existentes eran globales. Para no perder datos, se asignan
            // al primer hotel registrado. Si no hay hoteles, se crea uno por defecto.
            migrationBuilder.Sql(@"
                DECLARE @hotelId int = (SELECT MIN(Id) FROM Hoteles);
                IF @hotelId IS NULL
                BEGIN
                    INSERT INTO Hoteles (Nombre, CreatedDate) VALUES ('Hotel principal', GETUTCDATE());
                    SET @hotelId = SCOPE_IDENTITY();
                END
                UPDATE TiposHabitacion SET IdHotel = @hotelId WHERE IdHotel = 0;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_TiposHabitacion_IdHotel_Nombre",
                table: "TiposHabitacion",
                columns: new[] { "IdHotel", "Nombre" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TiposHabitacion_Hoteles_IdHotel",
                table: "TiposHabitacion",
                column: "IdHotel",
                principalTable: "Hoteles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TiposHabitacion_Hoteles_IdHotel",
                table: "TiposHabitacion");

            migrationBuilder.DropIndex(
                name: "IX_TiposHabitacion_IdHotel_Nombre",
                table: "TiposHabitacion");

            migrationBuilder.DropColumn(
                name: "IdHotel",
                table: "TiposHabitacion");

            migrationBuilder.CreateIndex(
                name: "IX_TiposHabitacion_Nombre",
                table: "TiposHabitacion",
                column: "Nombre",
                unique: true);
        }
    }
}

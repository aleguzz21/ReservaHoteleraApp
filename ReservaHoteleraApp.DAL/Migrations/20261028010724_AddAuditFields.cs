using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservaHoteleraApp.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Usuarios",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<int>(
                name: "CreatedUser",
                table: "Usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyDate",
                table: "Usuarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifyUser",
                table: "Usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "TiposHabitacion",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<int>(
                name: "CreatedUser",
                table: "TiposHabitacion",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyDate",
                table: "TiposHabitacion",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifyUser",
                table: "TiposHabitacion",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Reservas",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<int>(
                name: "CreatedUser",
                table: "Reservas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyDate",
                table: "Reservas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifyUser",
                table: "Reservas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "RegistrosActividad",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<int>(
                name: "CreatedUser",
                table: "RegistrosActividad",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyDate",
                table: "RegistrosActividad",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifyUser",
                table: "RegistrosActividad",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Pagos",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<int>(
                name: "CreatedUser",
                table: "Pagos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyDate",
                table: "Pagos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifyUser",
                table: "Pagos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Habitaciones",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<int>(
                name: "CreatedUser",
                table: "Habitaciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyDate",
                table: "Habitaciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifyUser",
                table: "Habitaciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Estadias",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<int>(
                name: "CreatedUser",
                table: "Estadias",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyDate",
                table: "Estadias",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifyUser",
                table: "Estadias",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Clientes",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<int>(
                name: "CreatedUser",
                table: "Clientes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyDate",
                table: "Clientes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifyUser",
                table: "Clientes",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "ModifyDate",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "ModifyUser",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "TiposHabitacion");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "TiposHabitacion");

            migrationBuilder.DropColumn(
                name: "ModifyDate",
                table: "TiposHabitacion");

            migrationBuilder.DropColumn(
                name: "ModifyUser",
                table: "TiposHabitacion");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "ModifyDate",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "ModifyUser",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "RegistrosActividad");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "RegistrosActividad");

            migrationBuilder.DropColumn(
                name: "ModifyDate",
                table: "RegistrosActividad");

            migrationBuilder.DropColumn(
                name: "ModifyUser",
                table: "RegistrosActividad");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "ModifyDate",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "ModifyUser",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Habitaciones");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "Habitaciones");

            migrationBuilder.DropColumn(
                name: "ModifyDate",
                table: "Habitaciones");

            migrationBuilder.DropColumn(
                name: "ModifyUser",
                table: "Habitaciones");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Estadias");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "Estadias");

            migrationBuilder.DropColumn(
                name: "ModifyDate",
                table: "Estadias");

            migrationBuilder.DropColumn(
                name: "ModifyUser",
                table: "Estadias");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "ModifyDate",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "ModifyUser",
                table: "Clientes");
        }
    }
}

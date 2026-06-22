using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaVendaVeiculos.Migrations
{
    /// <inheritdoc />
    public partial class LocadoraDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    valorPadraoDiaria = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cpf = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Fabricantes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fabricantes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Veiculos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    modelo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    anoFabricacao = table.Column<int>(type: "int", nullable: false),
                    quilometragemAtual = table.Column<int>(type: "int", nullable: false),
                    fabricanteId = table.Column<int>(type: "int", nullable: false),
                    categoriaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veiculos", x => x.id);
                    table.ForeignKey(
                        name: "FK_Veiculos_Categorias_categoriaId",
                        column: x => x.categoriaId,
                        principalTable: "Categorias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Veiculos_Fabricantes_fabricanteId",
                        column: x => x.fabricanteId,
                        principalTable: "Fabricantes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alugueis",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    clienteId = table.Column<int>(type: "int", nullable: false),
                    veiculoId = table.Column<int>(type: "int", nullable: false),
                    dataSaida = table.Column<DateTime>(type: "datetime2", nullable: false),
                    dataDevolucao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    kmInicial = table.Column<int>(type: "int", nullable: false),
                    kmFinal = table.Column<int>(type: "int", nullable: true),
                    valorDiaria = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    valorTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alugueis", x => x.id);
                    table.ForeignKey(
                        name: "FK_Alugueis_Clientes_clienteId",
                        column: x => x.clienteId,
                        principalTable: "Clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Alugueis_Veiculos_veiculoId",
                        column: x => x.veiculoId,
                        principalTable: "Veiculos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alugueis_clienteId",
                table: "Alugueis",
                column: "clienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Alugueis_veiculoId",
                table: "Alugueis",
                column: "veiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_cpf",
                table: "Clientes",
                column: "cpf",
                unique: true,
                filter: "[cpf] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Veiculos_categoriaId",
                table: "Veiculos",
                column: "categoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Veiculos_fabricanteId",
                table: "Veiculos",
                column: "fabricanteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alugueis");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Veiculos");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Fabricantes");
        }
    }
}

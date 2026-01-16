using Microsoft.AspNetCore.Mvc;
using LicenciaSistemas.Data;
using LicenciaSistemas.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace LicenciaSistemas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LicenciasController : ControllerBase
    {
        private readonly MySqlContext _db;

        public LicenciasController(MySqlContext db)
        {
            _db = db;
        }

        // 🔹 Listar todas las licencias
        [HttpGet]
        public IActionResult GetAll()
        {
            var lista = new List<Licencia>();
            using var conn = _db.GetConnection();
            conn.Open();

            var sql = "SELECT * FROM licencias";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Licencia
                {
                    Id = reader.GetInt32("id"),
                    NombreEmpresa = reader.GetString("nombre_empresa"),
                    CuotaPagar = reader.GetDecimal("cuota_pagar"),
                    CuotaPagada = reader.GetDecimal("cuota_pagada"),
                    NumeroHabilitacion = reader.GetString("numero_habilitacion"),
                    Habilitado = reader.GetInt32("habilitado")
                });
            }
            return Ok(lista);
        }

        // 🔹 Obtener por código
        [HttpGet("{codigo}")]
        public IActionResult GetByCodigo(string codigo)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var sql = "SELECT * FROM licencias WHERE numero_habilitacion=@codigo";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@codigo", codigo);
            using var reader = cmd.ExecuteReader();

            if (!reader.Read()) return NotFound();

            return Ok(new Licencia
            {
                Id = reader.GetInt32("id"),
                NombreEmpresa = reader.GetString("nombre_empresa"),
                CuotaPagar = reader.GetDecimal("cuota_pagar"),
                CuotaPagada = reader.GetDecimal("cuota_pagada"),
                NumeroHabilitacion = reader.GetString("numero_habilitacion"),
                Habilitado = reader.GetInt32("habilitado")
            });
        }

        // 🔹 Crear licencia
        [HttpPost]
        public IActionResult Create(Licencia lic)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var sql = @"INSERT INTO licencias
                        (nombre_empresa, cuota_pagar, cuota_pagada, numero_habilitacion, habilitado)
                        VALUES (@n, @cp, @cpg, @num, @h)";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@n", lic.NombreEmpresa);
            cmd.Parameters.AddWithValue("@cp", lic.CuotaPagar);
            cmd.Parameters.AddWithValue("@cpg", lic.CuotaPagada);
            cmd.Parameters.AddWithValue("@num", lic.NumeroHabilitacion);
            cmd.Parameters.AddWithValue("@h", lic.Habilitado);
            cmd.ExecuteNonQuery();

            return Ok(new { success = true, mensaje = "Licencia creada" });
        }

        // 🔹 Actualizar licencia
        [HttpPut("{id}")]
        public IActionResult Update(int id, Licencia lic)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var sql = @"UPDATE licencias SET
                        nombre_empresa=@n,
                        cuota_pagar=@cp,
                        cuota_pagada=@cpg,
                        numero_habilitacion=@num,
                        habilitado=@h
                        WHERE id=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@n", lic.NombreEmpresa);
            cmd.Parameters.AddWithValue("@cp", lic.CuotaPagar);
            cmd.Parameters.AddWithValue("@cpg", lic.CuotaPagada);
            cmd.Parameters.AddWithValue("@num", lic.NumeroHabilitacion);
            cmd.Parameters.AddWithValue("@h", lic.Habilitado);
            cmd.Parameters.AddWithValue("@id", id);

            if (cmd.ExecuteNonQuery() == 0) return NotFound();
            return Ok(new { success = true, mensaje = "Licencia actualizada" });
        }

        // 🔹 Eliminar licencia
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var sql = "DELETE FROM licencias WHERE id=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            if (cmd.ExecuteNonQuery() == 0) return NotFound();
            return Ok(new { success = true, mensaje = "Licencia eliminada" });
        }

        // 🔹 Verificar licencia
        [HttpGet("verificar/{codigo}")]
        public IActionResult Verificar(string codigo)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var sql = "SELECT habilitado FROM licencias WHERE numero_habilitacion=@codigo LIMIT 1";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@codigo", codigo);

            var result = cmd.ExecuteScalar();
            if (result == null) return NotFound(new { success = false });

            bool habilitado = Convert.ToInt32(result) == 1;
            return Ok(new { success = true, habilitado });
        }
    }
}

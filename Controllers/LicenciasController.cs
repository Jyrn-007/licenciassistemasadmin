using Microsoft.AspNetCore.Mvc;
using LicenciaSistemas.Data;
using LicenciaSistemas.Models;
using MySql.Data.MySqlClient;

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

        // 🔍 LISTAR TODAS LAS LICENCIAS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Licencia> lista = new();

            using var conn = _db.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM licencias";
            MySqlCommand cmd = new(sql, conn);

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

        // 🔍 OBTENER LICENCIA POR CÓDIGO
        [HttpGet("{codigo}")]
        public IActionResult GetByCodigo(string codigo)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM licencias WHERE numero_habilitacion = @codigo";
            MySqlCommand cmd = new(sql, conn);
            cmd.Parameters.AddWithValue("@codigo", codigo);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return NotFound();

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

        // ➕ CREAR LICENCIA
        [HttpPost]
        public IActionResult Create(Licencia lic)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string sql = @"INSERT INTO licencias
                (nombre_empresa, cuota_pagar, cuota_pagada, numero_habilitacion, habilitado)
                VALUES (@n, @cp, @cpg, @num, @h)";

            MySqlCommand cmd = new(sql, conn);
            cmd.Parameters.AddWithValue("@n", lic.NombreEmpresa);
            cmd.Parameters.AddWithValue("@cp", lic.CuotaPagar);
            cmd.Parameters.AddWithValue("@cpg", lic.CuotaPagada);
            cmd.Parameters.AddWithValue("@num", lic.NumeroHabilitacion);
            cmd.Parameters.AddWithValue("@h", lic.Habilitado);

            cmd.ExecuteNonQuery();

            return Ok(new { success = true, mensaje = "Licencia creada" });
        }

        // ✏️ ACTUALIZAR LICENCIA
        [HttpPut("{id}")]
        public IActionResult Update(int id, Licencia lic)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string sql = @"UPDATE licencias SET
                nombre_empresa = @n,
                cuota_pagar = @cp,
                cuota_pagada = @cpg,
                numero_habilitacion = @num,
                habilitado = @h
                WHERE id = @id";

            MySqlCommand cmd = new(sql, conn);
            cmd.Parameters.AddWithValue("@n", lic.NombreEmpresa);
            cmd.Parameters.AddWithValue("@cp", lic.CuotaPagar);
            cmd.Parameters.AddWithValue("@cpg", lic.CuotaPagada);
            cmd.Parameters.AddWithValue("@num", lic.NumeroHabilitacion);
            cmd.Parameters.AddWithValue("@h", lic.Habilitado);
            cmd.Parameters.AddWithValue("@id", id);

            if (cmd.ExecuteNonQuery() == 0)
                return NotFound();

            return Ok(new { success = true, mensaje = "Licencia actualizada" });
        }

        // ❌ ELIMINAR LICENCIA
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string sql = "DELETE FROM licencias WHERE id = @id";
            MySqlCommand cmd = new(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            if (cmd.ExecuteNonQuery() == 0)
                return NotFound();

            return Ok(new { success = true, mensaje = "Licencia eliminada" });
        }



        // 🔐 VERIFICACIÓN PARA WPF

        [HttpGet("verificar/{codigo}")]
        public IActionResult Verificar(string codigo)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string sql = "SELECT habilitado FROM licencias WHERE numero_habilitacion=@codigo LIMIT 1";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@codigo", codigo);

            var result = cmd.ExecuteScalar();
            if (result == null) return NotFound(new { success = false });

            bool habilitado = Convert.ToInt32(result) == 1;

            return Ok(new { success = true, habilitado });
        }
    }
}

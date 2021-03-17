using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Incluir()
        {
            return View();
        }

        
        private void CarregarBeneficiarios(long idCliente)
        {
            BoBeneficiario boBeneficiario = new BoBeneficiario();
            List<BeneficiarioModel> lstBeneficiariosModel;

            if (Session["ListaBeneficiarios"] == null)
            {
                List<Beneficiario> lstBeneficiarios = boBeneficiario.Listar(idCliente);
                lstBeneficiariosModel = new List<BeneficiarioModel>();

                foreach (Beneficiario b in lstBeneficiarios)
                {
                    BeneficiarioModel model = new BeneficiarioModel();
                    model.Cpf = b.Cpf;
                    model.Nome = b.Nome;
                    model.Id = b.Id;
                    model.Acao = 0;
                    model.Posicao = lstBeneficiariosModel.Count;

                    lstBeneficiariosModel.Add(model);
                }
                
                Session.Add("ListaBeneficiarios", lstBeneficiariosModel);
            }
           
        }


        [HttpPost]
        public string InserirBeneficiario(string cpf, string nome)
        {
            List<BeneficiarioModel> lstBeneficiarios;

            BeneficiarioModel model = new BeneficiarioModel();
            model.Cpf = cpf;
            model.Nome = nome;
            model.Id = long.MinValue;
            model.Acao = 1;

            if (Session["ListaBeneficiarios"] == null)
            {
                lstBeneficiarios = new List<BeneficiarioModel>();
                Session.Add("ListaBeneficiarios", lstBeneficiarios);
            }
            else
            {
                lstBeneficiarios = (List<BeneficiarioModel>)Session["ListaBeneficiarios"];
            }

            if (FI.AtividadeEntrevista.Util.Valida.CPFValido(model.Cpf))
            {
                //Verifica se o CPF já foi cadastrado
                if (lstBeneficiarios.Where(x => x.Cpf == cpf && x.Acao != -1).ToList().Count == 0)
                {
                    model.Posicao = lstBeneficiarios.Count;
                    lstBeneficiarios.Add(model);
                }
                else
                {
                    return "O CPF já foi cadastrado para este cliente.";
                }

                return "OK";

            }
            else
            {
                return "CPF inválido.";
            }


        }


        [HttpPost]
        public string ExcluirBeneficiario(long posicao)
        {
            List<BeneficiarioModel> lstBeneficiarios = (List<BeneficiarioModel>)Session["ListaBeneficiarios"];

            BeneficiarioModel model = lstBeneficiarios.Where(x => x.Posicao == posicao).First();

            model.Acao = -1;

            return "OK";
        }



        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente bo = new BoCliente();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                if (FI.AtividadeEntrevista.Util.Valida.CPFValido(model.Cpf))
                {
                    model.Id = bo.Incluir(new Cliente()
                    {
                        CEP = model.CEP,
                        Cidade = model.Cidade,
                        Email = model.Email,
                        Estado = model.Estado,
                        Logradouro = model.Logradouro,
                        Nacionalidade = model.Nacionalidade,
                        Nome = model.Nome,
                        Sobrenome = model.Sobrenome,
                        Telefone = model.Telefone,
                        Cpf = model.Cpf
                    });

                    if (model.Id == long.MinValue)
                    {
                        Response.StatusCode = 400;
                        return Json("CPF já cadastrado");
                    }

                    SalvarBeneficiarios(model.Id);
                    Session.Remove("ListaBeneficiarios");
                    return Json("Cadastro efetuado com sucesso");
                }
                else
                {
                    Response.StatusCode = 400;
                    return Json("CPF inválido");
                }

                
            }
        }

        private void SalvarBeneficiarios(long idCliente)
        {
            BoBeneficiario boBeneficiario = new BoBeneficiario();

            List<BeneficiarioModel> lstBeneficiarios;

            if (Session["ListaBeneficiarios"] == null)
            {
                lstBeneficiarios = new List<BeneficiarioModel>();
            }
            else
            {
                lstBeneficiarios = (List<BeneficiarioModel>)Session["ListaBeneficiarios"];
            }

            foreach (BeneficiarioModel b in lstBeneficiarios)
            {
                if (b.Acao == -1)
                {
                    boBeneficiario.Excluir(b.Id);
                }
                else if (b.Acao != 0)
                {
                    Beneficiario item = new Beneficiario();
                    item.Cpf = b.Cpf;
                    item.Nome = b.Nome;
                    item.IdCliente = idCliente;

                    boBeneficiario.Incluir(item);
                }
            }
        }


        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente bo = new BoCliente();
            
            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                if (FI.AtividadeEntrevista.Util.Valida.CPFValido(model.Cpf))
                {
                    long idAlterado = bo.Alterar(new Cliente()
                    {
                        Id = model.Id,
                        CEP = model.CEP,
                        Cidade = model.Cidade,
                        Email = model.Email,
                        Estado = model.Estado,
                        Logradouro = model.Logradouro,
                        Nacionalidade = model.Nacionalidade,
                        Nome = model.Nome,
                        Sobrenome = model.Sobrenome,
                        Telefone = model.Telefone,
                        Cpf = model.Cpf
                    });


                    if (idAlterado == long.MinValue)
                    {
                        Response.StatusCode = 400;
                        return Json("CPF já cadastrado");
                    }

                    SalvarBeneficiarios(idAlterado);
                    return Json("Cadastro alterado com sucesso");
                }
                else
                {
                    Response.StatusCode = 400;
                    return Json("CPF inválido");
                }
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            Models.ClienteModel model = null;

            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone,
                    Cpf = cliente.Cpf
                };

            }

            CarregarBeneficiarios(id);

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                Session.Remove("ListaBeneficiarios");

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult PopularBeneficiarios(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                List<BeneficiarioModel> lstBeneficiarios;

                if (Session["ListaBeneficiarios"] == null)
                {
                    lstBeneficiarios = new List<BeneficiarioModel>();
                }
                else
                {
                    lstBeneficiarios = (List<BeneficiarioModel>)Session["ListaBeneficiarios"];
                }

                //Nao mostra registros que serao deletados
                lstBeneficiarios = lstBeneficiarios.Where(x => x.Acao != -1).ToList();

                //Return result to jTable
                return Json(new { Result = "OK", Records = lstBeneficiarios, TotalRecordCount = lstBeneficiarios.Count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }

}
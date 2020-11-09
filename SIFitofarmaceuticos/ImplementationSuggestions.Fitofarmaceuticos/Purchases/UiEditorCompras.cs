using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Primavera.Extensibility.BusinessEntities;
using Primavera.Extensibility.BusinessEntities.ExtensibilityService.EventArgs;
using Primavera.Extensibility.Purchases.Editors;
using StdBE100;

namespace Primavera.SIFitofarmaceuticos.Purchases
{
    public class UiEditorCompras : EditorCompras
    {
        public override void ValidaLinha(int NumLinha, ExtensibilityEventArgs e)
        {
            base.ValidaLinha(NumLinha, e);

            try
            {
                //Apenas valida documentos financeiros ou de transporte
                if ((BSO.Compras.TabCompras.DaValorAtributo(DocumentoCompra.Tipodoc, "TipoDocumento") < 3))
                    return;

                //Valida��es dos fitofarmac�uticos
                if ((bool)(BSO.Base.Artigos.DaValorAtributo(DocumentoCompra.Linhas.GetEdita(NumLinha).Artigo, "CDU_Fitofarmaceutico")))
                {

                    //Obrigat�rio indicar o n�mero de autoriza��o de venda
                    if (string.IsNullOrEmpty(DocumentoCompra.Linhas.GetEdita(NumLinha).CamposUtil["CDU_NumeroAutorizacao"].Valor.ToString()))
                        throw new Exception("O n�mero de autoriza��o de venda � obrigat�rio nos produtos fitofarmac�uticos.");

                    //Obrigat�rio indicar o lote
                    if (DocumentoCompra.Linhas.GetEdita(NumLinha).Lote.Equals("<L01>") || string.IsNullOrEmpty(DocumentoCompra.Linhas.GetEdita(NumLinha).Lote))
                        throw new Exception("� obrigat�rio indicar o lote dos produtos fitofarmac�uticos.");

                    //A compra a entidades n�o autorizadas implica a identifica��o do armaz�m origem
                    //O armaz�m origem tem de pertencer ao fornecedor
                    if (!(bool)(BSO.Base.Fornecedores.DaValorAtributo(DocumentoCompra.Entidade, "CDU_OperadorFitofarmaceuticos") ?? false))
                    {
                        //Se n�o foi definido nenhum armaz�m de proveniencia associa o primeiro do fornecedor (caso exista)
                        if (string.IsNullOrEmpty((DocumentoCompra.Linhas.GetEdita(NumLinha).CamposUtil["CDU_ArmazemProveniencia"].Valor ?? string.Empty).ToString()))
                        {
                            StdBELista LstArmProveniencia = BSO.Consulta(string.Format("select top(1) [CDU_Codigo] from TDU_ArmazensProveniencia where [CDU_CodFornecedor] = '{0}'", DocumentoCompra.Entidade));
                            if (!LstArmProveniencia.Vazia())
                            {
                                DocumentoCompra.Linhas.GetEdita(NumLinha).CamposUtil["CDU_ArmazemProveniencia"].Valor = LstArmProveniencia.Valor("CDU_Codigo");
                            }
                            else
                            {
                                throw new Exception("N�o existe nenhum armaz�m de proveni�ncia associado a este fornecedor. A compra de produtos fitofarmac�uticos a entidades isentas de autoriza��o implica a identifica��o do armaz�m de proveni�ncia.");
                            }
                        }
                        
                        //Valida se o armaz�m de proveni�ncia pertence ao fornecedor
                        StdBECamposChave objCampoChave = new StdBECamposChave();
                        objCampoChave.AddCampoChave("CDU_Codigo", DocumentoCompra.Linhas.GetEdita(NumLinha).CamposUtil["CDU_ArmazemProveniencia"].Valor);

                        if (!BSO.TabelasUtilizador.DaValorAtributo("TDU_ArmazensProveniencia", objCampoChave, "CDU_CodFornecedor").Equals(DocumentoCompra.Entidade))
                            throw new Exception("O armaz�m de proveni�ncia n�o pertence � entidade do documento.");
                    }
                }
            }
            catch (Exception ex)
            {
                PSO.Dialogos.MostraErro(ex.Message, StdPlatBS100.StdBSTipos.IconId.PRI_Exclama);
                DocumentoCompra.Linhas.Remove(NumLinha);
            }
        }
    }
}

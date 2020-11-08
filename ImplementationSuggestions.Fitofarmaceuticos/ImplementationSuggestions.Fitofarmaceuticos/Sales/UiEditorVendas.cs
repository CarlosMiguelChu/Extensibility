using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Primavera.Extensibility.BusinessEntities;
using Primavera.Extensibility.BusinessEntities.ExtensibilityService.EventArgs;
using Primavera.Extensibility.Sales.Editors;

namespace DEMO.Sales
{
    public class UiEditorVendas : EditorVendas
    {
        public override void ValidaLinha(int NumLinha, ExtensibilityEventArgs e)
        {
            base.ValidaLinha(NumLinha, e);

            try
            {
                //Apenas valida documentos de encomenda, de transporte ou financeiros
                if (BSO.Vendas.TabVendas.DaValorAtributo(DocumentoVenda.Tipodoc, "TipoDocumento") < 2)
                    return;

                //Valida��es dos fitofarmac�uticos
                if ((bool)(BSO.Base.Artigos.DaValorAtributo(DocumentoVenda.Linhas.GetEdita(NumLinha).Artigo, "CDU_Fitofarmaceutico") ?? false))
                {
                    //Obrigat�rio indicar o n�mero de autoriza��o de venda
                    if (string.IsNullOrEmpty(DocumentoVenda.Linhas.GetEdita(NumLinha).CamposUtil["CDU_NumeroAutorizacao"].Valor.ToString()))
                        throw new Exception("O n�mero de autoriza��o de venda � obrigat�rio nos produtos fitofarmac�uticos.");

                    //A entidade tem de ser autorizada
                    if (string.IsNullOrEmpty(DocumentoVenda.CamposUtil["CDU_NumeroOperador"].Valor.ToString()))
                        throw new Exception("Os produtos fitofarmac�uticos s� podem ser vendidos a entidades autorizadas.");
                }
            }
            catch (Exception ex)
            {
                PSO.Dialogos.MostraErro(ex.Message, StdPlatBS100.StdBSTipos.IconId.PRI_Exclama);
                DocumentoVenda.Linhas.Remove(NumLinha);
            }
        }

    }
}

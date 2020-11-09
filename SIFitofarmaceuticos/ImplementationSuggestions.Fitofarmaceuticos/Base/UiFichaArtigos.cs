using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Primavera.Extensibility.BusinessEntities;
using Primavera.Extensibility.Base.Editors;
using Primavera.Extensibility.BusinessEntities.ExtensibilityService.EventArgs;

namespace Primavera.SIFitofarmaceuticos.Base
{
    public class UiFichaArtigos : FichaArtigos
    {
        public override void AntesDeGravar(ref bool Cancel, ExtensibilityEventArgs e)
        {
            base.AntesDeGravar(ref Cancel, e);

            try
            {
                if ((bool)this.Artigo.CamposUtil["CDU_Fitofarmaceutico"].Valor)
                {
                    if (string.IsNullOrEmpty(this.Artigo.CamposUtil["CDU_FitofarmaceuticoNumAut"].Valor.ToString()))
                    {
                        throw new Exception("O n�mero de autoriza��o � obrigat�rio nos produtos fitofarmac�uticos.");
                    }

                    if (!this.Artigo.TrataLotes)
                    {
                        throw new Exception("O tratamento de lotes � obrigat�rio nos produtos fitofarmac�uticos.");
                    }
                }
            }
            catch (Exception ex)
            {
                PSO.Dialogos.MostraErro(ex.Message, StdPlatBS100.StdBSTipos.IconId.PRI_Exclama);
                Cancel = true;
            }
        }
    }
}

namespace testAspOracle01.persistence.Helpers {
    public class Param {
        public string ParamName { get; set; }
        public string ParamValue { get; set; }
        public ParamMeterTypeEnum ParamType { get; set; }

        public Param () {

        }
        public Param (string paramName, string paramValue, ParamMeterTypeEnum type) {
            this.ParamName = paramName;
            this.ParamValue = paramValue;
            this.ParamType = type;
        }

        public Param (string paramName, string paramValue) {
            this.ParamName = paramName;
            this.ParamValue = paramValue;
            this.ParamType = ParamMeterTypeEnum.STRING;
        }

        public Param (string paramName, int paramValue) {
            this.ParamName = paramName;
            this.ParamValue = paramValue.ToString ();
            this.ParamType = ParamMeterTypeEnum.INTEGER;
        }
    }
    public enum ParamMeterTypeEnum {
        STRING,
        INTEGER,
        DECIMAL,
        DATETIME,
        DATE
    }
}
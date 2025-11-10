import utilities from "../utils";
const boilerPlate = {
    init() {
        let options = window.dnn.initPbiEmbedded();

        utilities.init(options.utility);
        utilities.moduleName = options.moduleName;

    },
    dispatch() {
        throw new Error("dispatch method needs to be overwritten from the Redux store");
    }
};


export default boilerPlate;
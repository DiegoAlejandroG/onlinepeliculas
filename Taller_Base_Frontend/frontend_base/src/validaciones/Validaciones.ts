export function primeraLetraMayuscula(){
    return {
        name: 'primera-letra-mayuscula',
        message: 'La primera letra debe ser mayúscula',
        test: (valor: string | undefined) =>{
            if (valor && valor.length > 0){
                const primeraLetra = valor.substring(0, 1);
                return primeraLetra === primeraLetra.toUpperCase();
            }

            return true;
        }
    }
}

export function fechaNoPuedeSerFutura(){
    return {
        name: 'fecha-no-es-futura',
        message: 'La fecha no puede ser del futuro',
        test: (valor: string | undefined) => {
            if (!valor) return true;

            const fecha = new Date(valor);
            const hoy = new Date();
            return fecha <= hoy;
        }
    }
}

export function fechaNoAnteriorA1900(){
    return {
        name: 'fecha-no-anterior-1900',
        message: 'La fecha no puede ser anterior a 1900',
        test: (valor: string | undefined) => {
            if (!valor) return true;

            const fecha = new Date(valor);
            const limite = new Date('1900-01-01');
            return fecha >= limite;
        }
    }
}

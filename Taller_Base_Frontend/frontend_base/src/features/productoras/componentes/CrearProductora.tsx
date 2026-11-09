import type { SubmitHandler } from "react-hook-form";
import FormularioProductora from "./FormularioProductora";
import type ProductoraCreacion from "../modelos/ProductoraCreacion.model";
import { useNavigate } from "react-router";
import { useState } from "react";
import axios from "axios";
import clienteAPI from "../../../api/clienteAxios";
import { extraerErrores } from "../../../utilidades/extraerErrores";
import convertirProductoraAFormData from "../utilidades/convertirProductoraAFormData";

export default function CrearProductora() {
    const navigate = useNavigate();
    const [errores, setErrores] = useState<string[]>([]);

    const onSubmit: SubmitHandler<ProductoraCreacion> = async (valores) => {
        setErrores([]);

        try {
            const formData = convertirProductoraAFormData(valores);
            await clienteAPI.post('/productoras', formData, {
                headers: { 'Content-Type': 'multipart/form-data' }
            });
            navigate('/productoras');
        } catch (error) {
            if (axios.isAxiosError(error)) {
                const data: unknown = error.response?.data;
                if (typeof data === 'object' && data !== null && 'errors' in data) {
                    setErrores(extraerErrores(error));
                } else if (error.response) {
                    setErrores([`Error ${error.response.status}: ${error.message}`]);
                } else {
                    setErrores([`No se pudo conectar con el API: ${error.message}`]);
                }
            } else {
                setErrores([error instanceof Error ? error.message : 'Ocurrió un error inesperado al crear la productora.']);
            }
        }
    }

    return (
        <>
            <h3>Crear Productora</h3>
            <FormularioProductora errores={errores} onSubmit={onSubmit} />
        </>
    )
}

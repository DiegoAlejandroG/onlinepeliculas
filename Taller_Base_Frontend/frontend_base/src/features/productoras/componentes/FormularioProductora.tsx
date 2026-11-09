import { useEffect, useState, type ChangeEvent } from "react";
import { useForm, type SubmitHandler } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { NavLink } from "react-router";
import type ProductoraCreacion from "../modelos/ProductoraCreacion.model";
import Boton from "../../../componentes/Boton";
import { fechaNoAnteriorA1900, fechaNoPuedeSerFutura, primeraLetraMayuscula } from "../../../validaciones/Validaciones";
import MostrarErrores from "../../../componentes/MostrarErrores";

export default function FormularioProductora(props: FormularioProductoraProps) {

    const {
        register,
        handleSubmit,
        setValue,
        formState: { errors, isValid, isSubmitting }
    } = useForm<ProductoraCreacion>({
        resolver: yupResolver(reglasDeValidacion),
        mode: 'onChange',
        defaultValues: props.modelo ?? { nombre: '', sitioWeb: '' }
    });

    const [logoPreviewURL, setLogoPreviewURL] = useState<string>();
    const [errorLogo, setErrorLogo] = useState<string>();

    useEffect(() => {
        return () => {
            if (logoPreviewURL) {
                URL.revokeObjectURL(logoPreviewURL);
            }
        }
    }, [logoPreviewURL]);

    const manejarCambioLogo = (event: ChangeEvent<HTMLInputElement>) => {
        const archivo = event.currentTarget.files?.[0];
        if (!archivo) {
            return;
        }

        if (!['image/jpeg', 'image/png'].includes(archivo.type)) {
            setErrorLogo('El archivo debe ser una imagen jpg o png');
            return;
        }

        if (archivo.size > 4 * 1024 * 1024) {
            setErrorLogo('El peso del archivo no debe ser mayor a 4 MB');
            return;
        }

        setErrorLogo(undefined);
        setValue('logo', archivo, { shouldDirty: true, shouldValidate: true });
        setLogoPreviewURL(URL.createObjectURL(archivo));
    }

    const onSubmit: SubmitHandler<ProductoraCreacion> = async (data) => {
        if (errorLogo) {
            return;
        }

        await props.onSubmit(data);
    }

    const logoActualURL = typeof props.modelo?.logo === 'string' ? props.modelo.logo : undefined;
    const logoMostrar = logoPreviewURL ?? logoActualURL;

    return (
        <>
            <MostrarErrores errores={props.errores} />
            <form onSubmit={handleSubmit(onSubmit)}>
                <div className="form-group">
                    <label htmlFor="nombre">Nombre</label>
                    <input type="text" id="nombre" autoComplete="off" className="form-control" {...register('nombre')} />
                    {errors.nombre && <p className="error">{errors.nombre.message}</p>}
                </div>

                <div className="form-group">
                    <label htmlFor="fechaFundacion">Fecha de fundación</label>
                    <input type="date" id="fechaFundacion" className="form-control" {...register('fechaFundacion')} />
                    {errors.fechaFundacion && <p className="error">{errors.fechaFundacion.message}</p>}
                </div>

                <div className="form-group">
                    <label htmlFor="sitioWeb">Sitio web</label>
                    <input type="text" id="sitioWeb" autoComplete="off" className="form-control" {...register('sitioWeb')} />
                    {errors.sitioWeb && <p className="error">{errors.sitioWeb.message}</p>}
                </div>

                <div className="form-group">
                    <label htmlFor="logo">Logo</label>
                    <input
                        type="file"
                        id="logo"
                        accept="image/jpeg,image/png"
                        className="form-control"
                        onChange={manejarCambioLogo}
                    />
                    {errorLogo && <p className="error">{errorLogo}</p>}
                    {logoMostrar && <div className="mt-2">
                        <img src={logoMostrar} alt="Logo de la productora" style={{ maxWidth: 200, maxHeight: 200 }} />
                    </div>}
                </div>

                <div className="mt-2">
                    <Boton type="submit" disabled={!isValid || isSubmitting || Boolean(errorLogo)}>
                        {isSubmitting ? 'Enviando...' : 'Enviar'}</Boton>
                    <NavLink className="btn btn-secondary ms-2" to="/productoras">Cancelar</NavLink>
                </div>
            </form>
        </>
    )
}

interface FormularioProductoraProps {
    modelo?: ProductoraCreacion;
    onSubmit: SubmitHandler<ProductoraCreacion>;
    errores: string[];
}

const reglasDeValidacion = yup.object({
    nombre: yup.string().required('El nombre es obligatorio').test(primeraLetraMayuscula()),
    fechaFundacion: yup.string().required('La fecha de fundación es obligatoria')
        .test(fechaNoPuedeSerFutura()).test(fechaNoAnteriorA1900()),
    sitioWeb: yup.string().required('El sitio web es obligatorio').url('Debe ser una URL válida')
})

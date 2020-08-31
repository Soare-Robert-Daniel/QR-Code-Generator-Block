<?php
/**
 * Plugin Name:       Simple QR Code Generator Block
 * Plugin URI:        https://github.com/Soare-Robert-Daniel/QR-Code-Generator-Block
 * Description:       It generate a QR Code based on a giving text.
 * Version:           1.1.2
 * Requires at least: 5.2
 * Requires PHP:      7.2
 * Author:            Soare Robert Daniel
 * Author URI:         https://profiles.wordpress.org/soarerobertdaniel7/
 * License:           GPLv2 or later
 */

/**
 * Registers all block assets so that they can be enqueued through the block editor
 * in the corresponding context.
 *
 * @see https://developer.wordpress.org/block-editor/tutorials/block-tutorial/applying-styles-with-stylesheets/
 */
function simple_qr_code_generator_init() {
	$dir = dirname( __FILE__ );

	$script_asset_path = "$dir/build/index.asset.php";
	if ( ! file_exists( $script_asset_path ) ) {
		throw new Error(
			'You need to run `npm start` or `npm run build` for the "plugins/qr-code-generator-block" block first.'
		);
	}
	$index_js     = 'build/index.js';
	$script_asset = require( $script_asset_path );
	wp_register_script(
		'simple-qr-code-generator-editor',
		plugins_url( $index_js, __FILE__ ),
		$script_asset['dependencies'],
		$script_asset['version']
	);

	$editor_css = 'build/index.css';
	wp_register_style(
		'simple-qr-code-generator-editor',
		plugins_url( $editor_css, __FILE__ ),
		array(),
		filemtime( "$dir/$editor_css" )
	);

	$style_css = 'build/style-index.css';
	wp_register_style(
		'simple-qr-code-generator',
		plugins_url( $style_css, __FILE__ ),
		array(),
		filemtime( "$dir/$style_css" )
	);

	register_block_type( 'soare-robert/simple-qr-code-generator-block', array(
		'editor_script' => 'simple-qr-code-generator-editor',
		'editor_style'  => 'simple-qr-code-generator-editor',
		'style'         => 'simple-qr-code-generator',
	) );
}
add_action( 'init', 'simple_qr_code_generator_init' );
